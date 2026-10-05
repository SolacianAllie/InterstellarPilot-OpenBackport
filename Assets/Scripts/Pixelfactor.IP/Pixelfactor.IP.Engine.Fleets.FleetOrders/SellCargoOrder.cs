using System.Collections.Generic;
using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;

namespace Pixelfactor.IP.Engine.Fleets.FleetOrders
{
	public class SellCargoOrder : FleetOrder
	{
		public bool CompleteWhenNoBuyerFound;

		public bool CompleteWhenNoCargoToSell = true;

		public float CustomSellCargoTime;

		public int FreeUnitsCompleteThreshold = -1;

		public CargoTrader ManualBuyer;

		public float MinBuyPriceMultiplier;

		private List<CargoClass> sellCargoClasses = new List<CargoClass>();

		public bool SellEquipment;

		public bool SellOnlyListedCargos;

		public List<CargoClass> SellCargoClasses => sellCargoClasses;

		public override FleetOrderType OrderType => FleetOrderType.SellCargo;

		public override string GetDescription()
		{
			if (sellCargoClasses.Count == 1)
			{
				return $"Sell {sellCargoClasses[0].ClassName}";
			}
			return "Sell cargo";
		}

		protected override ActiveFleetOrder createActiveFleetOrder()
		{
			ActiveSellCargoOrder activeSellCargoOrder = gameObject.AddComponent<ActiveSellCargoOrder>();
			activeSellCargoOrder.SellCargoObjective = this;
			return activeSellCargoOrder;
		}

		public override bool IsRepeatable()
		{
			return true;
		}
	}
}
