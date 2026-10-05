using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.Engine.Triggers
{
	public class Trigger_Scenario_AllowDialog : TriggerBase
	{
		public override TriggerType Type => TriggerType.Scenario_AllowDialog;

		protected override bool evaluate(EngineASX engine)
		{
			return engine.CanShowDialog();
		}
	}
}
