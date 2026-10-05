using System.Collections.Generic;
using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.Unity.Utils;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Comms.FactionDynamicComms
{
	public class DynamicCommsWelcomeMessageCalculator : MonoBehaviour
	{
		public string GetMessage(ICommsController commsController)
		{
			DynamicCommsSettings dynamicCommsSettings = GameController.Instance.GameSettings.DynamicCommsSettings;
			List<DynamicCommsProfile> profiles = ((commsController.NumStagesShownFromCommsHandler <= 1) ? dynamicCommsSettings.WelcomeMessageProfiles : dynamicCommsSettings.RepeatWelcomeMessageProfiles);
			Faction ownerFaction = commsController.CommsHandler.OwnerFaction;
			return DynamicCommsMessageSubstitutions.Substitute(DynamicCommsProfileFinder.GetProfile(EngineASX.Instance.LocalFaction, ownerFaction, profiles).Messages.GetRandom(), EngineASX.Instance.LocalPlayer.Person, ownerFaction);
		}
	}
}
