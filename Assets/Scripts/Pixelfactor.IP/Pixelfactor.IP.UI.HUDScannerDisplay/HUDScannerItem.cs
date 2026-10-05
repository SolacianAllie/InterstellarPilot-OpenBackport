using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.UI.HUDScannerDisplay
{
	public class HUDScannerItem
	{
		public Unit Unit { get; set; }

		public float Distance { get; set; }

		public bool IsCustomWaypoint { get; set; }

		public bool IsMissionWaypoint { get; set; }
	}
}
