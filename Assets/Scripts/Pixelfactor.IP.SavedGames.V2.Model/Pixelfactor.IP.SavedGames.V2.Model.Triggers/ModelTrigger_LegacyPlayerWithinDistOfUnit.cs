using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_LegacyPlayerWithinDistOfUnit : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Player_NearUnit;

		public float Distance { get; set; } = 200f;

		public ModelUnit TargetUnit { get; set; }
	}
}
