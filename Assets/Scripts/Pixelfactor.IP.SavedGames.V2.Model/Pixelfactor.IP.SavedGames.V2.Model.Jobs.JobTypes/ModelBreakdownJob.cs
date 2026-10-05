using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.SavedGames.V2.Model.Jobs.JobTypes
{
	public class ModelBreakdownJob : ModelJob
	{
		public override JobType JobType => JobType.Breakdown;

		public ModelSector BreakdownDestinationSector { get; set; }

		public Vec3 BreakdownDestinationPosition { get; set; }

		public ModelUnitClass BreakdownUnitClass { get; set; }
	}
}
