using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Player_HasOpenedMessage : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Player_HasOpenedMessage;

		public ModelPlayerMessage Message { get; set; }
	}
}
