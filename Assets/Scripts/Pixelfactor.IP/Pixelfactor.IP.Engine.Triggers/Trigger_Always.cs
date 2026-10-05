using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.Engine.Triggers
{
	public class Trigger_Always : TriggerBase
	{
		public override TriggerType Type => TriggerType.Always;

		protected override bool evaluate(EngineASX engine)
		{
			return true;
		}
	}
}
