using System.Collections.Generic;
using System.Linq;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.Factions;

namespace Pixelfactor.IP.UI
{
	public class DockedShipItemList : ScrollList<UnitComponentHolder>
	{
		public UnitHangar Hangar;

		public Faction LocalFaction;

		protected override void OnRefreshing()
		{
			List<UnitComponentHolder> list = new List<UnitComponentHolder>();
			if (Hangar != null)
			{
				foreach (UnitComponentHolder item in from e in Hangar.DockedShips
					orderby !e.Unit.IsPlayerCurrentUnit, !e.Unit.IsOwnedByPlayer, e.ShipName
					select e)
				{
					list.Add(item);
				}
			}
			SetItems(list);
			for (int num = 0; num < ActiveItems.Count; num++)
			{
				((DockedShipItemUI)UIItemPool[num]).LocalFaction = LocalFaction;
			}
			base.OnRefreshing();
		}
	}
}
