using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Unit_ReceivedDamage : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Unit_ReceivedDamage;

		public ModelUnit TargetUnit { get; set; }
	}
}
