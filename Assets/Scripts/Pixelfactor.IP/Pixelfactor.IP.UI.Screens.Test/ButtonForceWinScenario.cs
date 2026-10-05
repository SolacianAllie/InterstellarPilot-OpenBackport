using Pixelfactor.IP.Testing;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Screens.Test
{
	public class ButtonForceWinScenario : MonoBehaviour
	{
		private Button button;

		private void Awake()
		{
			button = GetComponent<Button>();
			button.onClick.AddListener(OnClick);
		}

		public void OnClick()
		{
			TestHelper.ForceWinScenario();
		}
	}
}
