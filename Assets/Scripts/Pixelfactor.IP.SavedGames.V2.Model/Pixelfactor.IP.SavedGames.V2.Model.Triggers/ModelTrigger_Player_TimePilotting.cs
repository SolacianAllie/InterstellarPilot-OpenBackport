using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Player_TimePilotting : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Player_TimePilotting;

		public float Time { get; set; } = 4f;
	}
}
