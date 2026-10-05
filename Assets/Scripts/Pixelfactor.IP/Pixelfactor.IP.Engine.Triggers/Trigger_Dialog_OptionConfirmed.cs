using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.Engine.Comms;

namespace Pixelfactor.IP.Engine.Triggers
{
	public class Trigger_Dialog_OptionConfirmed : TriggerBase
	{
		private int lastTimesEntered;

		public DialogOption Option;

		public override TriggerType Type => TriggerType.Dialog_OptionConfirmed;

		protected override bool evaluate(EngineASX engine)
		{
			if (Option.TimesEntered > lastTimesEntered)
			{
				lastTimesEntered = Option.TimesEntered;
				return true;
			}
			return false;
		}

		private void OnEnable()
		{
			lastTimesEntered = Option.TimesEntered;
		}
	}
}
