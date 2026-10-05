using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelUndockOrder : ModelFleetOrder
	{
		public override FleetOrderType OrderType => FleetOrderType.Undock;
	}
}
