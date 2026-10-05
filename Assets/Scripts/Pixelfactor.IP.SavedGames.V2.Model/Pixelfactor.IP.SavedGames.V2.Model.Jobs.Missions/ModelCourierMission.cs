using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.SavedGames.V2.Model.Jobs.Missions
{
	public class ModelCourierMission : ModelMission
	{
		public override MissionType MissionType => MissionType.Courier;

		public ModelUnit PickupUnit { get; set; }

		public ModelUnit DestinationUnit { get; set; }

		public ModelComponentUnitCargoDataItem CargoItem { get; set; }

		public bool HasPlayerPickedUpCargo { get; set; }
	}
}
