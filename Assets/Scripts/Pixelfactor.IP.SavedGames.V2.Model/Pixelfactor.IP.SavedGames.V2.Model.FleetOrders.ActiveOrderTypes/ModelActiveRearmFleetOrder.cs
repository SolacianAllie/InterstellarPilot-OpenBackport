using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.ActiveOrderTypes
{
	public class ModelActiveRearmFleetOrder : ModelActiveFleetOrder
	{
		public ActiveRearmFleetOrderState State { get; set; }

		public ModelUnit CurrentRearmLocationUnit { get; set; }
	}
}
