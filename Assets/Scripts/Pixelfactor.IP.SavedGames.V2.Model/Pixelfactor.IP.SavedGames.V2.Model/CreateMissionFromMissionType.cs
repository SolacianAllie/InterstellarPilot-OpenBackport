using Pixelfactor.IP.Common;
using Pixelfactor.IP.SavedGames.V2.Model.Jobs;
using Pixelfactor.IP.SavedGames.V2.Model.Jobs.Missions;

namespace Pixelfactor.IP.SavedGames.V2.Model
{
	public static class CreateMissionFromMissionType
	{
		public static ModelMission Create(MissionType jobType)
		{
			return jobType switch
			{
				MissionType.DeliverShip => new ModelDeliverShipMission(), 
				MissionType.Courier => new ModelCourierMission(), 
				MissionType.DestroyGroup => new ModelDestroyUnitsMission(), 
				MissionType.Breakdown => new ModelBreakdownMission(), 
				_ => new ModelMission(), 
			};
		}
	}
}
