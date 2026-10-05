using Pixelfactor.IP.Engine.Hypersleep;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Hypersleep
{
	[RequireComponent(typeof(Button))]
	public class HypersleepButton : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(ButtonClick);
		}

		private void ButtonClick()
		{
			HypersleepHelper.TryEnterHypersleep();
		}
	}
}
