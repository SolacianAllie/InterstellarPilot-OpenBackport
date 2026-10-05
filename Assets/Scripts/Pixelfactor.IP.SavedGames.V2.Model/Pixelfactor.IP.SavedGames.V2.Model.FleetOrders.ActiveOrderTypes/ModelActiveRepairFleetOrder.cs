using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.ActiveOrderTypes
{
	public class ModelActiveRepairFleetOrder : ModelActiveFleetOrder
	{
		public ActiveRepairFleetOrderState RepairState { get; set; }

		public ModelUnit CurrentRepairLocationUnit { get; set; }
	}
}
