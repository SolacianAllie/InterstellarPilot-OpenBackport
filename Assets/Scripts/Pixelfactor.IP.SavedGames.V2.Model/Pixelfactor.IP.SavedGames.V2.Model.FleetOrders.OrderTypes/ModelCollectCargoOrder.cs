using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelCollectCargoOrder : ModelFleetOrder
	{
		public ModelUnit TargetUnit { get; set; }

		public override FleetOrderType OrderType => FleetOrderType.CollectCargo;
	}
}
