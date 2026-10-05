using Pixelfactor.IP.Engine;
using Pixelfactor.IP.UI.Screens.UniverseMap;

namespace Pixelfactor.IP.UI.Screens.Orders.Buttons
{
	public class ExploreSectorOrderButton : OrderButton
	{
		protected override bool ShouldBeInteractable()
		{
			if (base.ShouldBeInteractable())
			{
				return OrderedFleetIsMobile;
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
				universeMap.CenterOnOrderTargetSector(OrderTarget);
				universeMap.RestrictNavigationAway();
			});
		}

		private void SectorPicked(UniverseMapScreen sender, Sector selectedSector)
		{
			if (selectedSector != null)
			{
				OnOrderIssuing(out var stack);
				OrdersHelper.OrderExploreSector(OrderTarget, selectedSector, stack);
				OnOrderIssued();
			}
			else
			{
				OnOrderCancelled();
			}
		}
	}
}
