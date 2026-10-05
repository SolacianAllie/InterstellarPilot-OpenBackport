using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.ActiveOrderTypes
{
	public class ModelActiveSellCargoOrder : ModelActiveFleetOrder
	{
		public double SellExpireTime { get; set; }

		public ModelCargoClass SellCargoClass { get; set; }

		public ActiveSellCargoOrderState State { get; set; }

		public ModelUnit TraderTargetUnit { get; set; }
	}
}
