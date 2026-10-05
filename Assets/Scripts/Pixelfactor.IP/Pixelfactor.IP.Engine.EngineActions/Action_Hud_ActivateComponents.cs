using System;
using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.UI.Screens.Hud;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Hud_ActivateComponents : EngineAction
	{
		public bool ShowComponents;

		public override ActionType Type => ActionType.Hud_ActivateComponents;

		public override void Execute()
		{
			base.Execute();
			foreach (HudComponent value in Enum.GetValues(typeof(HudComponent)))
			{
				engine.Hud.SetHudComponentVisible(value, ShowComponents);
			}
		}
	}
}
