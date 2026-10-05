using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.Engine.Triggers
{
	public class Trigger_Player_IsPilotting : TriggerBase
	{
		public bool WaitForHud = true;

		public override TriggerType Type => TriggerType.Player_IsPilotting;

		protected override bool evaluate(EngineASX engine)
		{
			if (engine.LocalPlayer != null && engine.LocalPlayer.Person.IsPilot)
			{
				if (WaitForHud)
				{
					return engine.IsPlayerPilotAndHudCurrentPanel;
				}
				return true;
			}
			return false;
		}
	}
}
