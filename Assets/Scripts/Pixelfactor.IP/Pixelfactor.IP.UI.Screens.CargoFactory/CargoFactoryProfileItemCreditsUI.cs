using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.CargoFactory;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Screens.CargoFactory
{
	public class CargoFactoryProfileItemCreditsUI : MonoBehaviour
	{
		public Text Text;

		private CargoFactoryProfileItem cargoFactoryProfileItem;

		public CargoFactoryProfileItem CargoFactoryProfileItem
		{
			get
			{
				return cargoFactoryProfileItem;
			}
			set
			{
				cargoFactoryProfileItem = value;
			}
		}

		public void Refresh()
		{
			if (cargoFactoryProfileItem != null)
			{
				Text.text = TextFormattingHelper.FormatCredits(CalculateRevenueCredits());
			}
		}

		private int CalculateRevenueCredits()
		{
			int num = 0;
			foreach (CargoFactoryProfileItemInput input in cargoFactoryProfileItem.Inputs)
			{
				int num2 = input.CalculateConsumerRevenue(EngineASX.Instance.EconomySettings, cargoFactoryProfileItem.RevenueMultiplier);
				num += num2;
			}
			return num;
		}
	}
}
