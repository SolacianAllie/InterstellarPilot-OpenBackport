using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Player_InSector : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Player_InSector;

		public ModelSector Sector { get; set; }
	}
}
