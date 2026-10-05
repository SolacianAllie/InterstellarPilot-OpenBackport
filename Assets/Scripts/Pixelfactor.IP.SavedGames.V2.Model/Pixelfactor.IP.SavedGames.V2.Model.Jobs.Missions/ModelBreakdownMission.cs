using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.SavedGames.V2.Model.Jobs.Missions
{
	public class ModelBreakdownMission : ModelMission
	{
		public override MissionType MissionType => MissionType.Breakdown;

		public ModelUnit BaseUnit { get; set; }

		public ModelUnit BreakdownUnit { get; set; }
	}
}
