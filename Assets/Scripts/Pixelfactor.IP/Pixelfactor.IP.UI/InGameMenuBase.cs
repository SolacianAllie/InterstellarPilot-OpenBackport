using Pixelfactor.IP.Engine;
using Pixelfactor.IP.UI.Screens.Hud;
using UnityEngine;

namespace Pixelfactor.IP.UI
{
	public class InGameMenuBase : MonoBehaviour
	{
		private InGameMenuBase backMenu;

		public InGameMenuControllerUI MenuController;

		public bool AllowAutoClose = true;

		public Unit PlayerUnit => Hud.PlayerUnit;

		public HudScreen Hud => MenuController.Hud;

		public InGameMenuBase BackMenu
		{
			get
			{
				return backMenu;
			}
			set
			{
				backMenu = value;
			}
		}

		private void OnEnable()
		{
			MenuController.OpenNewMenu(this);
		}
	}
}
