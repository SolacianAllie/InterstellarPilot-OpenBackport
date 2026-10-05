using System.Collections.Generic;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.UnitComponents;
using Pixelfactor.IP.UI.Screens;

namespace Pixelfactor.IP.UI
{
	public class ShipComponentsItemList : ScrollList<ComponentBay>
	{
		public void SetItemsFromUnitComponents(UnitComponentHolder unitComponents)
		{
			IEnumerable<ComponentBay> unitVisibleBays = ShipComponentsHelper.GetUnitVisibleBays(unitComponents.Unit);
			SetItems(unitVisibleBays);
		}
	}
}
