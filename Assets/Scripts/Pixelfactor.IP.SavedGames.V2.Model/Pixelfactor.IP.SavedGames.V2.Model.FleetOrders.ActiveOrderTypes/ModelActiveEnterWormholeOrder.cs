using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.ActiveOrderTypes
{
	public class ModelActiveEnterWormholeOrder : ModelActiveFleetOrder
	{
		public EnterWormholeState State { get; set; }
	}
}
