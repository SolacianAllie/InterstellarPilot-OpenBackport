using System.Collections.Generic;
using System.Linq;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.UnitComponents;

namespace Pixelfactor.IP.UI.Screens
{
	public static class UIHelper
	{
		public static CargoBayItem[] GetCargoItems(CargoBayComponent cargoBayComponent)
		{
			return (from e in cargoBayComponent.GetCargoItems()
				orderby e.Load descending
				select e).ToArray();
		}

		public static CargoBayItem[] GetCargoItems(Fleet fleet)
		{
			Dictionary<int, CargoBayItem> dictionary = new Dictionary<int, CargoBayItem>();
			foreach (UnitComponentHolder ship in fleet.Ships)
			{
				if (!(ship.CargoBayComponent != null))
				{
					continue;
				}
				CargoBayItem[] cargoItems = ship.CargoBayComponent.GetCargoItems();
				foreach (CargoBayItem cargoBayItem in cargoItems)
				{
					if (!dictionary.TryGetValue(cargoBayItem.CargoClass.UniqueId, out var value))
					{
						dictionary[cargoBayItem.CargoClass.UniqueId] = new CargoBayItem
						{
							CargoClass = cargoBayItem.CargoClass,
							Quantity = cargoBayItem.Quantity
						};
					}
					else
					{
						value.Quantity += cargoBayItem.Quantity;
					}
				}
			}
			return dictionary.Values.OrderByDescending((CargoBayItem e) => e.Load).ToArray();
		}
	}
}
