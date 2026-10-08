using UnityEngine;

namespace OpenFrontier.IP.Discord
{
	/// <summary>
	/// Discord Social SDK configuration. Create one asset and fill in
	/// the Application ID (Client ID) of the game's Discord application
	/// from https://discord.com/developers/applications.
	/// Rich Presence stays offline while the ID is unset.
	/// </summary>
	[CreateAssetMenu(fileName = "DiscordPresenceConfig", menuName = "Open Frontier/Discord Presence Config")]
	public class DiscordPresenceConfig : ScriptableObject
	{
		[Tooltip("The Discord application's Client ID (Application ID).")]
		public ulong ApplicationId;
	}
}
