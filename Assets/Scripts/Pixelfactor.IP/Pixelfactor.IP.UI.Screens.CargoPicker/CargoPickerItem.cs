using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.UI.Screens.CargoPicker
{
	public class CargoPickerItem
	{
		public CargoClass CargoClass { get; set; }

		public int MinQuantiy { get; set; }

		public int MaxQuantity { get; set; }

		public string AvailableText { get; set; } = "available";
	}
}
