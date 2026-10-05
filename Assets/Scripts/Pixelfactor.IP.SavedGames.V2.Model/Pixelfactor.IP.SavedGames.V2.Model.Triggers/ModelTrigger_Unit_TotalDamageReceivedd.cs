using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Unit_TotalDamageReceivedd : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Unit_TotalDamageReceived;

		public float Damage { get; set; }

		public ComparisonOp Operator { get; set; }

		public ModelUnit TargetUnit { get; set; }
	}
}
