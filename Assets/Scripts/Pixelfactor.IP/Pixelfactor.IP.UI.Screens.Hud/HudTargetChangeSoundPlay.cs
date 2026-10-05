using Pixelfactor.IP.Engine;
using UnityEngine;

namespace Pixelfactor.IP.UI.Screens.Hud
{
	public class HudTargetChangeSoundPlay : MonoBehaviour
	{
		public AudioClip AudioClip;

		public HudScreen HudScreen;

		private Unit lastTarget;

		private void Update()
		{
			Unit currentTarget = HudScreen.CurrentTarget;
			if (currentTarget != lastTarget)
			{
				if (currentTarget != null && GameController.Instance.PlayButtonSounds && AudioClip != null)
				{
					AudioHelper.PlaySound(AudioClip);
				}
				lastTarget = currentTarget;
			}
		}
	}
}
