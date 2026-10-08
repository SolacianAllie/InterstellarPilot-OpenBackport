using System;
using UnityEngine;
using Discord.Sdk;
using OpenFrontier.IP.Engine;
using OpenFrontier.IP.Engine.Factions;

namespace OpenFrontier.IP.Discord
{
	/// <summary>
	/// Discord Rich Presence for Open Frontier, powered by the Discord
	/// Social SDK (com.discord.partnersdk).
	///
	/// The SDK authenticates through a one-time device authorization
	/// flow: the first launch asks the player to approve the app with
	/// their Discord account, and the resulting tokens are stored in
	/// PlayerPrefs. Later launches refresh the stored token silently.
	/// If anything in that chain fails, presence simply stays offline -
	/// the game is never blocked on Discord.
	///
	/// The SDK pumps its own callbacks through the Unity player loop
	/// (see NativeMethods' static constructor in the package), so every
	/// callback below already runs on the main thread.
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
		private const float RefreshRetryCooldown = 60f;

		private Client client;
		private bool statusReady;
		private bool authInProgress;
		private bool reportedPresenceFailure;
		private string pendingCodeVerifier;
		private float lastRefreshAttempt = -RefreshRetryCooldown;
		private float nextPresenceUpdate;
		private ulong sessionStartEpoch;

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
			CreateClientAndAuthenticate();
		}

		private void Update()
		{
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
		// Authentication
		// ------------------------------------------------------------------

		private void CreateClientAndAuthenticate()
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
				return;
			}

			// Prefer a silent token refresh, then a stored access
			// token, and only fall back to the interactive device
			// authorization flow when nothing is stored.
			string refreshToken = PlayerPrefs.GetString(RefreshTokenKey, "");
			string accessToken = PlayerPrefs.GetString(AccessTokenKey, "");
			if (!string.IsNullOrEmpty(refreshToken))
			{
				RefreshAndConnect(refreshToken, () => ContinueAuthentication(accessToken));
			}
			else
			{
				ContinueAuthentication(accessToken);
			}
		}

		private void ContinueAuthentication(string accessToken)
		{
			if (client == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(accessToken))
			{
				client.UpdateToken(AuthorizationTokenType.Bearer, accessToken, result =>
				{
					if (result.Successful())
					{
						client.Connect();
					}
					else
					{
						StartDeviceAuthorization();
					}
				});
			}
			else
			{
				StartDeviceAuthorization();
			}
		}

		private void RefreshAndConnect(string refreshToken, Action onFailure)
		{
			if (client == null)
			{
				onFailure?.Invoke();
				return;
			}
			client.RefreshToken(config.ApplicationId, refreshToken,
				(result, accessToken, newRefreshToken, tokenType, expiresIn, scopes) =>
				{
					if (result.Successful() && !string.IsNullOrEmpty(accessToken))
					{
						StoreTokens(accessToken, newRefreshToken);
						client.UpdateToken(AuthorizationTokenType.Bearer, accessToken, updateResult =>
						{
							if (updateResult.Successful())
							{
								client.Connect();
							}
							else
							{
								onFailure?.Invoke();
							}
						});
					}
					else
					{
						onFailure?.Invoke();
					}
				});
		}

		private void StartDeviceAuthorization()
		{
			if (authInProgress || client == null)
			{
				return;
			}
			authInProgress = true;
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
				authInProgress = false;
				Debug.LogWarning("[Discord] Device authorization could not start: " + e.Message);
			}
		}

		private void OnAuthorizeCompleted(ClientResult result, string code, string redirectUri)
		{
			authInProgress = false;
			if (client == null)
			{
				return;
			}
			if (!result.Successful())
			{
				Debug.Log("[Discord] Authorization not completed (" + result.Error() + "). Presence stays offline until the next launch.");
				return;
			}
			client.GetToken(config.ApplicationId, code, pendingCodeVerifier, redirectUri,
				(tokenResult, accessToken, refreshToken, tokenType, expiresIn, scopes) =>
				{
					if (tokenResult.Successful() && !string.IsNullOrEmpty(accessToken))
					{
						StoreTokens(accessToken, refreshToken);
						client.UpdateToken(AuthorizationTokenType.Bearer, accessToken, updateResult =>
						{
							if (updateResult.Successful())
							{
								client.Connect();
							}
						});
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

		private void OnDiscordStatusChanged(Client.Status status, Client.Error error, int errorDetail)
		{
			statusReady = (status == Client.Status.Ready);
			switch (status)
			{
			case Client.Status.Ready:
				Debug.Log("[Discord] Connected - Rich Presence active.");
				PushPresence();
				break;
			case Client.Status.Disconnected:
				// A dropped socket with a stored refresh token is
				// usually an expired access token: refresh once,
				// throttled, instead of leaving presence dead.
				if (error != Client.Error.None && Time.time - lastRefreshAttempt > RefreshRetryCooldown)
				{
					string refreshToken = PlayerPrefs.GetString(RefreshTokenKey, "");
					if (!string.IsNullOrEmpty(refreshToken))
					{
						lastRefreshAttempt = Time.time;
						RefreshAndConnect(refreshToken, null);
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
