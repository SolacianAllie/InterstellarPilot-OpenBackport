using Pixelfactor.IP.Engine;
using TMPro;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Screens.CargoPicker
{
	public class CargoPickerListItem : ScrollListItem<CargoPickerItem>
	{
		public Image CargoIconImage;

		public TextMeshProUGUI NameLabel;

		public override void Refresh()
		{
			base.Refresh();
			CargoIconImage.sprite = EngineASX.Instance.EngineResources.GetCargoSpriteOrDefault(Item.CargoClass);
			NameLabel.text = GetNameText();
		}

		private string GetNameText()
		{
			return Item.CargoClass.ClassName + " (" + TextFormattingHelper.FormatCargoAmount(Item.MaxQuantity) + " " + Item.AvailableText + ")";
		}
	}
}
