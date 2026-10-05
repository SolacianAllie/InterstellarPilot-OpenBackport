using System;
using Pixelfactor.IP.Engine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Screens.UnitInfo
{
	public class HangarBayItem : ScrollListItem<UnitHangarBay>
	{
		public Text TitleLabel;

		public override void Refresh()
		{
			base.Refresh();
			TitleLabel.text = "Hangar Bay - " + Enum.GetName(typeof(ShipHullType), Item.MaxHullType);
		}
	}
}
