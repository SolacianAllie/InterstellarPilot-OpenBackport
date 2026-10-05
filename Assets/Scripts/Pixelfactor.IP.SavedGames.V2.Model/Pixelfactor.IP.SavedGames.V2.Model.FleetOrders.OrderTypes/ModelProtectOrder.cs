using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelProtectOrder : ModelMoveToOrder
	{
		public override FleetOrderType OrderType => FleetOrderType.Protect;
	}
}
