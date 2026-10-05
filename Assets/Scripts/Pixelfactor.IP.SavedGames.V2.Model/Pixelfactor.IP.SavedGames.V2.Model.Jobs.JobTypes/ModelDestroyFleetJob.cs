using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.SavedGames.V2.Model.Jobs.JobTypes
{
	public class ModelDestroyFleetJob : ModelJob
	{
		public override JobType JobType => JobType.DestroyGroup;

		public ModelFleetSpawnParams FleetSpawnParams { get; set; }
	}
}
