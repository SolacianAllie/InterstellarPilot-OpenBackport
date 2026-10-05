using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelAttackFleetOrder : ModelFleetOrder
	{
		public ModelFleet Target { get; set; }

		public float AttackPriority { get; set; }

		public override FleetOrderType OrderType => FleetOrderType.AttackGroup;
	}
}
