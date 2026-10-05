using Pixelfactor.IP.Engine.CargoFactory;

namespace Pixelfactor.IP.UI.Screens.CargoFactory
{
	public class CargoFactoryItemListItemUI : ScrollListItem<CargoFactoryItem>
	{
		public CargoFactoryProfileItemUI ProfileItemUI;

		public CargoFactoryItemStatusUI ItemStatusUI;

		public override void Refresh()
		{
			base.Refresh();
			ProfileItemUI.Refresh(Item.Profile);
			ItemStatusUI.Refresh(Item);
		}
	}
}
