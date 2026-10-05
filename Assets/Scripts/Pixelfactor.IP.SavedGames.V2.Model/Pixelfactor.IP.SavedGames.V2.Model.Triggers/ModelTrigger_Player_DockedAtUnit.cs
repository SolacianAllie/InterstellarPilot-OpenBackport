using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Player_DockedAtUnit : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Player_DockedAtUnit;

		public ModelUnit TargetUnit { get; set; }
	}
}
