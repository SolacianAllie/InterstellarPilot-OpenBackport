using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelTradeOrder : ModelFleetOrder
	{
		public int MinBuyQuantity { get; set; }

		public float MinBuyCargoPercentage { get; set; }

		public override FleetOrderType OrderType => FleetOrderType.Trade;
	}
}
