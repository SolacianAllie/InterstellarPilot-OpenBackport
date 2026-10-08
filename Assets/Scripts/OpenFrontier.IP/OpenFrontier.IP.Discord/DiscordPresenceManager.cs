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
	/// Two modes:
	///
	/// - Direct RPC (default): no authorization whatsoever. Naming
	///   the application is enough for the SDK to publish presence
	///   straight to the running Discord client (the Discord app on
	///   Android, SDK 1.10+). No prompt, no stored tokens, no portal
	///   OAuth configuration - presence simply appears whenever a
	///   Discord client is available.
	///
	/// - Authenticated: the SDK's device authorization flow, which
	///   shows presence even when no Discord client is running. The
	///   player authorizes the app once (tokens are stored in
	///   PlayerPrefs and refreshed silently afterwards), which
	///   requires the redirect URLs and "public client" setting on
	///   the Discord application.
	///
	/// Authentication, when enabled, runs as a chain with a
	/// watchdog: stored refresh token -> stored access token ->
	/// interactive re-prompt. Each stage that fails (including a
	/// stored token the Discord server has rejected, which only
	/// shows up as an async socket drop) advances to the next
	/// stage, so a player who loses their data, revokes the app,
	/// or switches accounts is re-prompted instead of silently
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

		[Tooltip("Direct mode: publish presence straight to the running Discord client over RPC, with no account authorization at all - no login prompt, no stored tokens, no portal OAuth setup. Presence appears whenever Discord is open (on Android, whenever the Discord app is installed and signed in). Disable to use the authenticated flow instead, which shows presence even with no Discord client running, but asks the player to authorize the app once.")]
		[SerializeField]
		private bool useDirectRpc = true;

		[Tooltip("Attempt to connect on startup. Players can opt out by setting PlayerPrefs 'DiscordPresenceEnabled' to 0.")]
		[SerializeField]
		private bool connectOnStartup = true;

		[Tooltip("How often the presence line is refreshed, in seconds.")]
		[SerializeField]
		private float updateInterval = 15f;

		[Tooltip("Activity asset name for the large image, exactly as uploaded to the application's Rich Presence assets in the Discord Developer Portal.")]
		[SerializeField]
		private string largeImageAsset = "openfrontier";

		[Tooltip("Optional activity asset name for the small image (the icon beside the presence text). Leave empty to show none. Upload e.g. 'ship' to your app's Rich Presence assets to use it.")]
		[SerializeField]
		private string smallImageAsset = "";

		[Tooltip("Activity asset name shown as the small image while the player's ship is docked. Upload an icon under this name to your app's Rich Presence assets.")]
		[SerializeField]
		private string dockedImageAsset = "docked";

		[Tooltip("Optional first presence button label (with its URL below). Leave either empty to show no buttons.")]
		[SerializeField]
		private string buttonOneLabel = "";

		[SerializeField]
		private string buttonOneUrl = "";

		[Tooltip("Optional second presence button label (with its URL below).")]
		[SerializeField]
		private string buttonTwoLabel = "";

		[SerializeField]
		private string buttonTwoUrl = "";

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
		private bool directRpcActive;
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

		/// <summary>True while the SDK reports the authenticated connection as Ready.</summary>
		public bool IsAuthorized => statusReady;

		/// <summary>True whenever presence can be published: the authenticated connection is Ready, or direct RPC mode is active.</summary>
		public bool IsPresenceLive => statusReady || directRpcActive;

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
			if (useDirectRpc)
			{
				StartDirectRpc();
			}
			else
			{
				BeginAuthentication();
			}
		}

		private void Update()
		{
			if (!directRpcActive)
			{
				UpdateAuthWatchdog();
			}
			if (client == null || (!statusReady && !directRpcActive))
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

		/// <summary>
		/// Direct RPC mode: no authorization at all. Naming the
		/// application is enough for the SDK to publish presence
		/// directly to the running Discord client (the Discord app
		/// on Android, SDK 1.10+) - nothing is prompted and nothing
		/// is stored, so there is no auth state to lose or recover.
		/// </summary>
		private void StartDirectRpc()
		{
			if (client == null)
			{
				return;
			}
			try
			{
				client.SetApplicationId(config.ApplicationId);
			}
			catch (Exception e)
			{
				Debug.LogWarning("[Discord] Direct RPC mode could not start: " + e.Message);
				return;
			}
			directRpcActive = true;
			client.IsDiscordAppInstalled(installed =>
			{
				if (installed)
				{
					Debug.Log("[Discord] Direct RPC mode - Rich Presence active (no authorization needed).");
				}
				else
				{
					Debug.Log("[Discord] No Discord client found - presence will appear once Discord is running.");
				}
			});
			PushPresence();
		}

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
			if (directRpcActive)
			{
				Debug.Log("[Discord] RestartAuthentication only applies to the authenticated flow - Direct RPC mode needs no authorization.");
				return;
			}
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
			if (client == null || (!statusReady && !directRpcActive))
			{
				return;
			}
			EngineASX engine = EngineASX.Instance;
			Unit ship = IsInMainMenu() ? null : GetPlayerShip(engine);
			Faction player = FindPlayerFaction(engine);
			Sector sector = engine != null ? engine.ActiveSector : null;
			string scenarioTitle = GetScenarioTitle(engine);
			int shipCount = CountShips(player);

			string details = IsInMainMenu()
				? "Cruising the open frontier"
				: BuildShipLine(ship);
			string state = BuildStateLine(sector, player, shipCount);

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
						assets.SetLargeText(string.IsNullOrEmpty(scenarioTitle)
							? "Open Frontier"
							: scenarioTitle);
						// The small image doubles as a docked flag -
						// one asset, two pieces of information.
						bool docked = ship != null && ship.IsDocked;
						if (!string.IsNullOrEmpty(smallImageAsset))
						{
							assets.SetSmallImage(smallImageAsset);
							assets.SetSmallText(docked ? "Docked" : "In flight");
						}
						else if (docked)
						{
							assets.SetSmallImage(dockedImageAsset);
							assets.SetSmallText("Docked");
						}
						activity.SetAssets(assets);
					}
					using (var timestamps = new ActivityTimestamps())
					{
						timestamps.SetStart(sessionStartEpoch);
						activity.SetTimestamps(timestamps);
					}
					AddButton(activity, buttonOneLabel, buttonOneUrl);
					AddButton(activity, buttonTwoLabel, buttonTwoUrl);
					client.UpdateRichPresence(activity, result =>
					{
						if (result.Successful())
						{
							reportedPresenceFailure = false;
						}
						else if (!reportedPresenceFailure)
						{
							reportedPresenceFailure = true;
							Debug.Log("[Discord] Presence update failed: " + result.Error()
								+ (directRpcActive ? " (no Discord client running?)" : ""));
						}
					});
				}
			}
			catch (Exception e)
			{
				Debug.LogWarning("[Discord] Presence update threw: " + e.Message);
			}
		}

		/// <summary>
		/// Details line: what the player is flying. A docked ship
		/// reads as such rather than pretending to fly.
		/// </summary>
		private static string BuildShipLine(Unit ship)
		{
			if (ship == null || !ship.IsValidAndNotDestroyed)
			{
				return "In flight";
			}
			string line = ship.IsDocked ? "Docked" : ship.GetClassAndSeriesName(true);
			if (!string.IsNullOrEmpty(ship.UnitName))
			{
				line += " \"" + ship.UnitName + "\"";
			}
			return line;
		}

		/// <summary>
		/// State line: where they are, who they are, what they
		/// command and what they're worth.
		/// </summary>
		private static string BuildStateLine(Sector sector, Faction player, int shipCount)
		{
			string line = sector != null ? sector.Name : "Open Frontier";
			if (player != null)
			{
				line += " - " + player.Name;
				if (shipCount > 0)
				{
					line += " - " + shipCount.ToString("N0") + " ship" + (shipCount == 1 ? "" : "s");
				}
				line += " - " + player.Credits.ToString("N0") + " cr";
			}
			return line;
		}

		private static string GetScenarioTitle(EngineASX engine)
		{
			if (engine == null || engine.World == null || engine.World.ScenarioInfo == null)
			{
				return string.Empty;
			}
			return engine.World.ScenarioInfo.Title;
		}

		private static int CountShips(Faction faction)
		{
			if (faction == null || faction.Fleets == null)
			{
				return 0;
			}
			int total = 0;
			for (int i = 0; i < faction.Fleets.Count; i++)
			{
				Fleet fleet = faction.Fleets[i];
				if (fleet != null && fleet.Ships != null)
				{
					total += fleet.Ships.Count;
				}
			}
			return total;
		}

		private static Unit GetPlayerShip(EngineASX engine)
		{
			if (engine == null)
			{
				return null;
			}
			Unit ship = engine.PlayerUnit;
			return ship != null && ship.IsValidAndNotDestroyed ? ship : null;
		}

		private static void AddButton(Activity activity, string label, string url)
		{
			if (string.IsNullOrEmpty(label) || string.IsNullOrEmpty(url))
			{
				return;
			}
			using (var button = new ActivityButton())
			{
				button.SetLabel(label);
				button.SetUrl(url);
				activity.AddButton(button);
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
