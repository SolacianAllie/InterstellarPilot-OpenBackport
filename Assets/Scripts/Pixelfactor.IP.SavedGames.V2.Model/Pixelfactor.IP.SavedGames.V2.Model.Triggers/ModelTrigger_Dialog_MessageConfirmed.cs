using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Dialog_MessageConfirmed : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Dialog_MessageConfirmed;

		public int DialogMessageId { get; set; }
	}
}
