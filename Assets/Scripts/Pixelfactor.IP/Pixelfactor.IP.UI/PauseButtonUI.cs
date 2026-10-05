using Pixelfactor.IP.Engine;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI
{
	public class PauseButtonUI : MonoBehaviour
	{
		private void Awake()
		{
			GetComponent<Button>().onClick.AddListener(button_Activated);
		}

		private void button_Activated()
		{
			EngineASX instance = EngineASX.Instance;
			instance.IsPaused = !instance.IsPaused;
		}
	}
}
