using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.UnitComponents;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI
{
	public class CapacitorSlider : MonoBehaviour
	{
		public CapacitorComponent Capacitor;

		public Text Label;

		public Slider Slider;

		public void Refresh()
		{
			if (Capacitor != null)
			{
				float charge = Capacitor.Charge;
				float capacity = Capacitor.CapacitorClass.Capacity;
				Slider.value = charge / capacity;
				Label.text = $"{TextFormattingHelper.FormatNumber(charge)} / {TextFormattingHelper.FormatNumber(capacity)}";
			}
		}
	}
}
