using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.SavedGames.V2.Model.Jobs.JobTypes
{
	public class ModelCourierJob : ModelJob
	{
		public override JobType JobType => JobType.Courier;

		public ModelUnit PickupUnit { get; set; }

		public ModelUnit DestinationUnit { get; set; }

		public ModelComponentUnitCargoDataItem Cargo { get; set; }
	}
}
