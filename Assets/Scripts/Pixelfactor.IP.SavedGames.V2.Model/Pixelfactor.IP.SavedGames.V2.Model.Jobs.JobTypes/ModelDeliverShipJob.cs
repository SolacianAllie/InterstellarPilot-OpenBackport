using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.SavedGames.V2.Model.Jobs.JobTypes
{
	public class ModelDeliverShipJob : ModelJob
	{
		public override JobType JobType => JobType.DeliverShip;

		public ModelUnitClass UnitClass { get; set; }

		public ModelUnit DestinationUnit { get; set; }
	}
}
