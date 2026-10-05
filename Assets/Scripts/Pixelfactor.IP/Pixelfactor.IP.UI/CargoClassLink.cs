using Pixelfactor.IP.Engine;
using UnityEngine;

namespace Pixelfactor.IP.UI
{
	public class CargoClassLink : MonoBehaviour
	{
		public CargoClass CargoClass;

		private void OnClick()
		{
			if (CargoClass != null)
			{
				UIController.Instance.ScreenNavigator.ShowCargoInfoScreen(CargoClass);
			}
		}
	}
}
