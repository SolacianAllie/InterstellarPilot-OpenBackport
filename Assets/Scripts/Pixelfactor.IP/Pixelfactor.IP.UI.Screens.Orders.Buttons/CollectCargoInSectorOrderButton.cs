using Pixelfactor.IP.Engine;
using Pixelfactor.IP.UI.Screens.UniverseMap;

namespace Pixelfactor.IP.UI.Screens.Orders.Buttons
{
	public class CollectCargoInSectorOrderButton : OrderButton
	{
		protected override bool ShouldBeInteractable()
		{
			if (base.ShouldBeInteractable() && OrderedFleetIsMobile)
			{
				return OrderTarget.AnyUnitHasTractorBeam;
			}
			return false;
		}

		protected override void OnButtonClick()
		{
			base.OnButtonClick();
			UIController.Instance.ScreenNavigator.ShowCustomUniverseMapScreen((UniverseMapScreen universeMap) =>
			{
				universeMap.Title = "Select Sector";
				universeMap.AllowSectorSelection = true;
				universeMap.EnabledSectors = OrdersHelper.GetPlayerUniverseMapPickableSectors();
				universeMap.ShowSelectedSectorInfo = false;
				universeMap.SectorSelectedCallback = SectorPicked;
				universeMap.SetSectorFromOrderTarget(OrderTarget);
				universeMap.RestrictNavigationAway();
			});
		}

		private void SectorPicked(UniverseMapScreen universeMapScreen, Sector sector)
		{
			universeMapScreen.SectorSelectedCallback = null;
			if (sector != null)
			{
				OnOrderIssuing(out var stack);
				OrdersHelper.OrderScavenge(OrderTarget, sector, stack);
				OnOrderIssued();
			}
			else
			{
				OnOrderCancelled();
			}
		}
	}
}
