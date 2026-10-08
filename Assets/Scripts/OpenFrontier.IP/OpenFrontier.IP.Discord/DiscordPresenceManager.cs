using System;
using UnityEngine;
using Discord.Sdk;
using OpenFrontier.IP.Engine;
using OpenFrontier.IP.Engine.Factions;

namespace OpenFrontier.IP.Discord
{
	/// <summary>
	/// Discord Rich Presence for Open Frontier, powered by the
	/// Discord Social SDK (com.discord.partnersdk).
	///
	/// The SDK authenticates through a device authorization
	/// flow: the first launch asks the player to approve the
	/// app with their Discord account, and the resulting
	/// tokens are stored in PlayerPrefs. Later launches detect
	/// the stored credentials and refresh them silently.
	///
	/// Authentication runs as a chain with a watchdog: stored
	/// refresh token -> stored access token -> interactive
	/// re-prompt. Each stage that fails (including a stored
	/// token the Discord server has rejected, which only shows
	/// up as an async socket drop) advances to the next stage,
	/// so a player who loses their data, revokes the app, or
	/// switches accounts is re-prompted instead of silently
	/// losing presence. A declined prompt is not nagged - the
	/// chain gives up for the launch and re-prompts next time.
	///
	/// The SDK pumps its own callbacks through the Unity player
	/// loop (see NativeMethods' static constructor in the
	/// package), so every callback below already runs on the
	/// main thread.
	/// </summary>
	public class DiscordPresenceManager : MonoBehaviour
	{
		[Tooltip("Discord application configuration (the Application ID from the Discord Developer Portal). Presence stays offline while this is unset.")]
		[SerializeField]
		private DiscordPresenceConfig config;

		[Tooltip("Attempt to connect on startup. Players can opt out by setting PlayerPrefs 'DiscordPresenceEnabled' to 0.")]
		[SerializeField]
		private bool connectOnStartup = true;

		[Tooltip("How often the presence line is refreshed, in seconds.")]
		[SerializeField]
		private float updateInterval = 15f;

		[Tooltip("Activity asset name for the large image, exactly as uploaded to the application's Rich Presence assets in the Discord Developer Portal.")]
		[SerializeField]
		private string largeImageAsset = "openfrontier";

		private const string EnabledKey = "DiscordPresenceEnabled";
		private const string AccessTokenKey = "DiscordAccessToken";
		private const string RefreshTokenKey = "DiscordRefreshToken";

		// Presence updates are throttled; a failure is logged once
		// so a persistent Discord outage can't flood the console.
		private const float MinUpdateInterval = 5f;

		// How long a token handshake may take before the auth
		// chain assumes it failed and advances.
		private const float TokenConnectTimeout = 10f;

		// How long the authorization prompt may go unanswered
		// (the player has to approve it in Discord) before the
		// chain gives up for this launch.
		private const float AuthorizationPromptTimeout = 120f;

		// Minimum spacing between silent recovery attempts while
		// the game runs, so a dead connection can't spam the
		// auth chain (or the re-prompt) every few seconds.
		private const float RecoveryCooldown = 60f;

		private enum AuthStage
		{
			Idle,
			TokenConnect,
			Prompting,
		}

		private Client client;
		private bool statusReady;
		private AuthStage authStage;
		private bool triedRefresh;
		private bool triedAccessToken;
		private bool triedPrompt;
		private float authStageDeadline = float.MaxValue;
		private float lastRecoveryAttempt = -RecoveryCooldown;
		private bool reportedPresenceFailure;
		private string pendingCodeVerifier;
		private float nextPresenceUpdate;
		private ulong sessionStartEpoch;

		/// <summary>True while the SDK reports the connection as Ready (authorized and live).</summary>
		public bool IsAuthorized => statusReady;

		/// <summary>True when credentials from a previous authorization are stored on this device.</summary>
		public bool HasStoredCredentials => !string.IsNullOrEmpty(PlayerPrefs.GetString(AccessTokenKey, ""));

		private void Awake()
		{
			sessionStartEpoch = (ulong)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
		}

		private void Start()
		{
			if (!connectOnStartup || PlayerPrefs.GetInt(EnabledKey, 1) == 0)
			{
				return;
			}
			if (config == null || config.ApplicationId == 0UL)
			{
				Debug.LogWarning("[Discord] Rich Presence is not configured: set an Application ID in the DiscordPresenceConfig asset. Presence will stay offline.");
				return;
			}
			CreateClient();
			BeginAuthentication();
		}

		private void Update()
		{
			UpdateAuthWatchdog();
			if (!statusReady || client == null)
			{
				return;
			}
			if (Time.time >= nextPresenceUpdate)
			{
				nextPresenceUpdate = Time.time + Math.Max(MinUpdateInterval, updateInterval);
				PushPresence();
			}
		}

		// ------------------------------------------------------------------
		// Authentication chain
		// ------------------------------------------------------------------

		private void CreateClient()
		{
			if (client != null)
			{
				return;
			}
			try
			{
				client = new Client(new ClientCreateOptions());
				client.SetStatusChangedCallback(OnDiscordStatusChanged);
			}
			catch (Exception e)
			{
				client = null;
				Debug.LogWarning("[Discord] Failed to create the Social SDK client: " + e.Message);
			}
		}

		/// <summary>
		/// (Re)runs the authentication chain from the beginning,
		/// remembering which stages already failed this launch.
		/// </summary>
		private void BeginAuthentication()
		{
			triedRefresh = false;
			triedAccessToken = false;
			triedPrompt = false;
			AdvanceAuthentication();
		}

		/// <summary>
		/// Clears any stored credentials and authenticates again -
		/// for players who want to switch accounts or recover from
		/// a rejected app authorization.
		/// </summary>
		public void RestartAuthentication()
		{
			PlayerPrefs.DeleteKey(AccessTokenKey);
			PlayerPrefs.DeleteKey(RefreshTokenKey);
			PlayerPrefs.Save();
			if (client != null)
			{
				try
				{
					client.Disconnect();
				}
				catch
				{
					// Re-authenticating - the old socket is irrelevant.
				}
			}
			statusReady = false;
			BeginAuthentication();
		}

		private void AdvanceAuthentication()
		{
			if (client == null)
			{
				return;
			}
			// Order: silent refresh, then the stored access
			// token, then the interactive re-prompt.
			if (!triedRefresh && HasStoredRefreshToken())
			{
				triedRefresh = true;
				RefreshAndConnect(
					PlayerPrefs.GetString(RefreshTokenKey, ""),
					AdvanceAuthentication);
				return;
			}
			if (!triedAccessToken && HasStoredCredentials)
			{
				triedAccessToken = true;
				ConnectWithStoredAccessToken();
				return;
			}
			if (!triedPrompt)
			{
				triedPrompt = true;
				StartDeviceAuthorization();
				return;
			}
			authStage = AuthStage.Idle;
			Debug.Log("[Discord] Authentication attempts exhausted; presence stays offline until the next launch.");
		}

		private static bool HasStoredRefreshToken()
		{
			return !string.IsNullOrEmpty(PlayerPrefs.GetString(RefreshTokenKey, ""));
		}

		private void RefreshAndConnect(string refreshToken, Action onFailure)
		{
			client.RefreshToken(config.ApplicationId, refreshToken,
				(result, accessToken, newRefreshToken, tokenType, expiresIn, scopes) =>
				{
					if (result.Successful() && !string.IsNullOrEmpty(accessToken))
					{
						StoreTokens(accessToken, newRefreshToken);
						ConnectWithAccessToken(accessToken);
					}
					else
					{
						onFailure?.Invoke();
					}
				});
		}

		private void ConnectWithStoredAccessToken()
		{
			client.UpdateToken(AuthorizationTokenType.Bearer,
				PlayerPrefs.GetString(AccessTokenKey, ""),
				result =>
				{
					if (result.Successful())
					{
						ConnectWithAccessToken(null);
					}
					else
					{
						AdvanceAuthentication();
					}
				});
		}

		/// <summary>
		/// Hands the (already stored) access token to the SDK and
		/// opens the socket. The token's validity only shows up
		/// asynchronously, so the watchdog watches for Ready - or
		/// a socket drop - and advances the chain on failure.
		/// </summary>
		private void ConnectWithAccessToken(string accessToken)
		{
			Action connect = () =>
			{
				client.Connect();
				authStage = AuthStage.TokenConnect;
				authStageDeadline = Time.time + TokenConnectTimeout;
			};
			if (!string.IsNullOrEmpty(accessToken))
			{
				// The socket must not open before the token is
				// set, so connect from inside the UpdateToken
				// callback (the SDK sample's pattern).
				client.UpdateToken(AuthorizationTokenType.Bearer, accessToken, _ => connect());
			}
			else
			{
				connect();
			}
		}

		private void StartDeviceAuthorization()
		{
			authStage = AuthStage.Prompting;
			authStageDeadline = Time.time + AuthorizationPromptTimeout;
			try
			{
				AuthorizationCodeVerifier verifier = client.CreateAuthorizationCodeVerifier();
				pendingCodeVerifier = verifier.Verifier();
				var args = new AuthorizationArgs();
				args.SetClientId(config.ApplicationId);
				args.SetScopes(Client.GetDefaultCommunicationScopes());
				args.SetCodeChallenge(verifier.Challenge());
				client.Authorize(args, OnAuthorizeCompleted);
			}
			catch (Exception e)
			{
				authStage = AuthStage.Idle;
				Debug.LogWarning("[Discord] Device authorization could not start: " + e.Message);
			}
		}

		private void OnAuthorizeCompleted(ClientResult result, string code, string redirectUri)
		{
			if (authStage == AuthStage.Prompting)
			{
				authStage = AuthStage.Idle;
				authStageDeadline = float.MaxValue;
			}
			if (client == null)
			{
				return;
			}
			if (!result.Successful())
			{
				// Declined or timed out: no nag this launch -
				// the chain restarts (and re-prompts) next time.
				Debug.Log("[Discord] Authorization not completed (" + result.Error() + "). Presence stays offline until the next launch.");
				return;
			}
			client.GetToken(config.ApplicationId, code, pendingCodeVerifier, redirectUri,
				(tokenResult, accessToken, refreshToken, tokenType, expiresIn, scopes) =>
				{
					if (tokenResult.Successful() && !string.IsNullOrEmpty(accessToken))
					{
						StoreTokens(accessToken, refreshToken);
						ConnectWithAccessToken(accessToken);
					}
					else
					{
						Debug.LogWarning("[Discord] Token exchange failed; presence stays offline.");
					}
				});
		}

		private void StoreTokens(string accessToken, string refreshToken)
		{
			PlayerPrefs.SetString(AccessTokenKey, accessToken);
			if (!string.IsNullOrEmpty(refreshToken))
			{
				PlayerPrefs.SetString(RefreshTokenKey, refreshToken);
			}
			PlayerPrefs.Save();
		}

		/// <summary>
		/// The chain's watchdog: a token handshake that neither
		/// reaches Ready nor drops within the timeout has failed,
		/// so the chain moves on (eventually to the re-prompt).
		/// </summary>
		private void UpdateAuthWatchdog()
		{
			if (authStage != AuthStage.Idle && Time.time >= authStageDeadline)
			{
				if (authStage == AuthStage.Prompting)
				{
					// The prompt went unanswered: give up for
					// this launch rather than nagging.
					authStage = AuthStage.Idle;
					authStageDeadline = float.MaxValue;
					Debug.Log("[Discord] Authorization prompt unanswered; presence stays offline until the next launch.");
				}
				else
				{
					AdvanceAuthentication();
				}
			}
		}

		private void OnDiscordStatusChanged(Client.Status status, Client.Error error, int errorDetail)
		{
			bool wasReady = statusReady;
			statusReady = (status == Client.Status.Ready);
			switch (status)
			{
			case Client.Status.Ready:
				authStage = AuthStage.Idle;
				authStageDeadline = float.MaxValue;
				Debug.Log("[Discord] Connected - Rich Presence active.");
				PushPresence();
				break;
			case Client.Status.Disconnected:
				if (authStage == AuthStage.TokenConnect)
				{
					// The stored token was rejected (or the
					// socket never came up): advance the chain.
					AdvanceAuthentication();
				}
				else if (wasReady && error != Client.Error.None
				         && Time.time - lastRecoveryAttempt > RecoveryCooldown)
				{
					// Was authorized and the connection dropped
					// (expired token, revoked app, network blip):
					// refresh silently first; on failure the full
					// chain runs, re-prompting if needed.
					lastRecoveryAttempt = Time.time;
					if (HasStoredRefreshToken())
					{
						RefreshAndConnect(
							PlayerPrefs.GetString(RefreshTokenKey, ""),
							AdvanceAuthentication);
					}
					else
					{
						AdvanceAuthentication();
					}
				}
				break;
			}
		}

		// ------------------------------------------------------------------
		// Presence
		// ------------------------------------------------------------------

		private void PushPresence()
		{
			if (client == null || !statusReady)
			{
				return;
			}
			string details;
			string state;
			if (IsInMainMenu())
			{
				details = "Cruising the open frontier";
				state = "Main menu";
			}
			else
			{
				EngineASX engine = EngineASX.Instance;
				Sector sector = engine != null ? engine.ActiveSector : null;
				Faction player = FindPlayerFaction(engine);
				details = sector != null ? sector.Name : "Open Frontier";
				state = player != null
					? player.Name + " - " + player.Credits.ToString("N0") + " credits"
					: "In flight";
			}

			try
			{
				using (var activity = new Activity())
				{
					activity.SetName("Open Frontier");
					activity.SetType(ActivityTypes.Playing);
					activity.SetSupportedPlatforms(ActivityGamePlatforms.Desktop | ActivityGamePlatforms.Android);
					activity.SetDetails(details);
					activity.SetState(state);
					using (var assets = new ActivityAssets())
					{
						assets.SetLargeImage(largeImageAsset);
						assets.SetLargeText("Open Frontier");
						activity.SetAssets(assets);
					}
					using (var timestamps = new ActivityTimestamps())
					{
						timestamps.SetStart(sessionStartEpoch);
						activity.SetTimestamps(timestamps);
					}
					client.UpdateRichPresence(activity, result =>
					{
						if (result.Successful())
						{
							reportedPresenceFailure = false;
						}
						else if (!reportedPresenceFailure)
						{
							reportedPresenceFailure = true;
							Debug.Log("[Discord] Presence update failed: " + result.Error());
						}
					});
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("[Discord] Presence update threw: " + e.Message);
			}
		}

		private static bool IsInMainMenu()
		{
			EngineASX engine = EngineASX.Instance;
			GameController gameController = GameController.Instance;
			if (engine == null || engine.World == null || gameController == null)
			{
				return false;
			}
			ScenarioInfo scenario = engine.World.ScenarioInfo;
			ScenarioInfo mainMenuScenario = gameController.MainMenuScenario;
			return scenario != null && mainMenuScenario != null
				&& (scenario == mainMenuScenario || scenario.UniqueId == mainMenuScenario.UniqueId);
		}

		private static Faction FindPlayerFaction(EngineASX engine)
		{
			if (engine == null || engine.Factions == null)
			{
				return null;
			}
			for (int i = 0; i < engine.Factions.Count; i++)
			{
				Faction faction = engine.Factions[i];
				if (faction != null && faction.IsPlayerFaction)
				{
					return faction;
				}
			}
			return null;
		}

		private void TearDown()
		{
			if (client != null)
			{
				try
				{
					client.Disconnect();
					client.Dispose();
				}
				catch
				{
					// Shutting down - the native library is going away.
				}
				client = null;
			}
		}

		private void OnApplicationQuit()
		{
			TearDown();
		}

		private void OnDestroy()
		{
			TearDown();
		}
	}
}
