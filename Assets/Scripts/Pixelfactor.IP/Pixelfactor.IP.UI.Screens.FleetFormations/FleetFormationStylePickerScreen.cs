using System;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.Fleets.FleetFormations;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Screens.FleetFormations
{
	public class FleetFormationStylePickerScreen : EngineScreen
	{
		public Action<FleetFormationStylePickerScreen, FleetFormationStyle> Callback;

		public FleetFormationStyleList FleetFormationStyleList;

		public Button OkButton;

		public FleetFormationStyle DefaultItem;

		protected override void awake()
		{
			base.awake();
			OkButton.onClick.AddListener(OkButtonClick);
		}

		protected override void refresh()
		{
			base.refresh();
			FleetFormationStyleList.SetItems(GameController.Instance.GameSettings.FormationSettings.FormationStyles);
			if (DefaultItem != null)
			{
				FleetFormationStyleList.FirstSelectedItem = DefaultItem;
			}
		}

		private void OkButtonClick()
		{
			if (Callback != null)
			{
				Callback(this, FleetFormationStyleList.FirstSelectedItem);
			}
		}
	}
}
