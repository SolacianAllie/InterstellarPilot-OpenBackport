using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.ActiveOrderTypes
{
	public class ModelActiveUniversePassengerTransportOrder : ModelActiveFleetOrder
	{
		public ModelPassengerGroup PassengerGroup { get; set; }

		public double EndBuySellTime { get; set; }

		public double LastStateChangeTime { get; set; }

		public ActiveTransportPassengerOrderState CurrentState { get; set; }
	}
}
