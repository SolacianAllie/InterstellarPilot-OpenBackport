using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelClaimUnitOrder : ModelFleetOrder
	{
		public ModelUnit Unit { get; set; }

		public override FleetOrderType OrderType => FleetOrderType.ClaimUnit;
	}
}
