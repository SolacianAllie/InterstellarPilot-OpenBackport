using Pixelfactor.IP.Common.FleetOrders;

namespace Pixelfactor.IP.SavedGames.V2.Model.FleetOrders.OrderTypes
{
	public class ModelManualRearmFleetOrder : ModelFleetOrder
	{
		public InsufficientCreditsMode InsufficientCreditsMode { get; set; }

		public ModelUnit RearmLocationUnit { get; set; }

		public float EquipmentCargoUsage { get; set; } = 0.25f;

		public override FleetOrderType OrderType => FleetOrderType.ManualRearm;
	}
}
