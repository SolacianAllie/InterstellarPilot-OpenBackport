using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Player_CurrentHudTarget : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Player_CurrentHudTarget;

		public ModelUnit TargetUnit { get; set; }
	}
}
