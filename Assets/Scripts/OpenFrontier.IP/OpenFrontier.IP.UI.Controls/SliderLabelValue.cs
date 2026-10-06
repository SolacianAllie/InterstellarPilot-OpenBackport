using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenFrontier.IP.UI.Controls
{
	[RequireComponent(typeof(Slider))]
	public class SliderLabelValue : MonoBehaviour
	{
		public string CustomFormat;

		public TextMeshProUGUI Label;

		private void Awake()
		{
			Slider slider = GetComponent<Slider>();
			slider.onValueChanged.AddListener((float value) =>
			{
				RefreshLabel(slider, value);
			});
			RefreshLabel(slider, slider.value);
		}

		private void RefreshLabel(Slider slider, float value)
		{
			if (!string.IsNullOrWhiteSpace(CustomFormat))
			{
				Label.text = value.ToString(CustomFormat);
			}
			else if (slider.wholeNumbers)
			{
				Label.text = $"{value:N0}";
			}
			else
			{
				Label.text = $"{value:N1}";
			}
		}
	}
}
