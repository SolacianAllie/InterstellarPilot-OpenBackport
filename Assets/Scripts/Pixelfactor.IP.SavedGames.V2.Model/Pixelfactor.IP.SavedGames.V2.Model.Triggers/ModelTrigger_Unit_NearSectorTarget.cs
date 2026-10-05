using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Triggers
{
	public class ModelTrigger_Unit_NearSectorTarget : ModelTrigger
	{
		public override TriggerType Type => TriggerType.Unit_NearSectorTarget;

		public float Distance { get; set; } = 200f;

		public ModelUnit TargetUnit { get; set; }

		public ModelSectorTarget SectorTarget { get; set; }
	}
}
