using System.Collections.Generic;
using Pixelfactor.IP.Engine.Factions;

namespace Pixelfactor.IP.Engine.Comms.FactionDynamicComms.Options.Mercenary
{
	public static class MercenaryOptionsController
	{
		public static void PopulateOptions(DynamicCommsHandler commsHandler, List<ICommsStageOption> stageOptionCache)
		{
			if (commsHandler.OwnerPilot != null)
			{
				Fleet fleet = commsHandler.OwnerPilot.GetFleet();
				if (fleet != null && commsHandler.OwnerFaction.FactionAI != null && FactionAIMercenary.CanFleetBeHiredAsMercenaryByPerson(commsHandler.OwnerFaction.FactionAI, fleet, EngineASX.Instance.LocalPlayer.Person))
				{
					HireMercenaryOption item = new HireMercenaryOption
					{
						OptionText = "Hire you...",
						OptionGroup = null
					};
					stageOptionCache.Add(item);
				}
			}
		}
	}
}
