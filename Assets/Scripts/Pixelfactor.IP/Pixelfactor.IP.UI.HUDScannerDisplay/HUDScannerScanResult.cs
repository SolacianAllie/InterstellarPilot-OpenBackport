using System.Collections.Generic;

namespace Pixelfactor.IP.UI.HUDScannerDisplay
{
	public struct HUDScannerScanResult
	{
		public HUDScannerScanFilterResult FilterResult { get; set; }

		public IEnumerable<HUDScannerItem> Items { get; set; }
	}
}
