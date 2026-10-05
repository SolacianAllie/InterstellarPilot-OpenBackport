using Pixelfactor.IP.Engine;
using Pixelfactor.IP.UI.Screens.Hud;
using UnityEngine;

namespace Pixelfactor.IP.UI
{
	public class TurnButtonUI : MonoBehaviour
	{
		private RepeatableButton button;

		private HudScreen Hud;

		public float TurnMultiplier = 1f;

		private void Start()
		{
			Hud = HudScreen.Instance;
			button = gameObject.GetComponent<RepeatableButton>();
		}

		private void Update()
		{
			if (EngineASX.LoadedAndReady && Hud.AllowPlayerSteering && button.IsPressed && Hud.PlayerUnit != null && Hud.PlayerUnit.ActiveUnit != null && Hud.PlayerUnit.ActiveUnit.ActiveUnitShip != null)
			{
				Hud.PlayerUnit.ActiveUnit.ActiveUnitShip.DesiredTurn += TurnMultiplier * Hud.TurnButtonSensitivity * Time.deltaTime;
			}
		}
	}
}
