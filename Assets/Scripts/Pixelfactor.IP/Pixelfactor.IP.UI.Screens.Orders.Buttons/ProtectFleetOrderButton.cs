using System.Collections.Generic;
using System.Linq;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.UI.Screens.FleetPicker;
using Pixelfactor.IP.UI.Screens.Fleets;

namespace Pixelfactor.IP.UI.Screens.Orders.Buttons
{
	public class ProtectFleetOrderButton : OrderButton
	{
		protected override bool ShouldBeInteractable()
		{
			if (base.ShouldBeInteractable())
			{
				return GetPickableFleets().Count() > 0;
			}
			return false;
		}

		protected override void OnButtonClick()
		{
			base.OnButtonClick();
			UIController.Instance.ScreenNavigator.ShowFleetPicker(GetPickableFleets().ToList(), OnFleetPicked, allowNone: false, allowCreateNew: false, "Select fleet to protect...");
		}

		private void OnFleetPicked(FleetPickerScreen sender, bool result, FleetPickerItem pickedFleet)
		{
			if (result && !OrderTarget.IsFleetOrdered(pickedFleet.Fleet))
			{
				SectorTarget target = new SectorTarget
				{
					TargetFleet = pickedFleet.Fleet
				};
				OnOrderIssuing(out var stack);
				OrdersHelper.OrderProtectTarget(OrderTarget, target, stack);
				OnOrderIssued();
			}
		}

		private IEnumerable<Fleet> GetPickableFleets()
		{
			return from e in FleetsHelper.GetOrderedPlayerFleets()
				where !OrderTarget.IsFleetOrdered(e)
				select e;
		}
	}
}
