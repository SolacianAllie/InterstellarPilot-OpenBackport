using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.HUDScannerDisplay
{
	public class HUDScannerFilterToggle : MonoBehaviour
	{
		public HUDScannerFilterType HUDScannerFilterType = HUDScannerFilterType.Ships;

		public Toggle Toggle;

		public GameObject AvailableObject;
	}
}
