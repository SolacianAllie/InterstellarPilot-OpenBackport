using TMPro;
using UnityEngine;

namespace Pixelfactor.IP.UI.Screens.Test.Timescale
{
	public class TimescaleOptions : MonoBehaviour
	{
		public TextMeshProUGUI CurrentTimeMultiplierLabel;

		private void Update()
		{
			CurrentTimeMultiplierLabel.text = $"{Time.timeScale:N2}";
		}
	}
}
