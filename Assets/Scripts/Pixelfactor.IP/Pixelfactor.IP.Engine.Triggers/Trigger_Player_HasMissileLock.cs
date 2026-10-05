using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.Engine.Triggers
{
	public class Trigger_Player_HasMissileLock : TriggerBase
	{
		public override TriggerType Type => TriggerType.Player_HasMissileLock;

		protected override bool evaluate(EngineASX engine)
		{
			if (engine.LocalPlayer != null && engine.IsPlayerPilot)
			{
				return engine.MissileLockController.HasMissileLock(engine.PlayerUnit);
			}
			return false;
		}
	}
}
