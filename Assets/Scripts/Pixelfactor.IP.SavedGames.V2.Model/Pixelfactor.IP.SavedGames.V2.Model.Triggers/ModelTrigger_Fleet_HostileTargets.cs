using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Fleet_HostileTargets : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Fleet_HostileTargets;

		public ModelFleet Fleet { get; set; }
	}
}
