using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.Engine.Comms;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Dialog_ChangeStage : EngineAction
	{
		public DialogStage Stage;

		public override ActionType Type => ActionType.Dialog_ChangeStage;

		public override void Execute()
		{
			base.Execute();
			engine.DialogController.ChangeStage(Stage);
		}
	}
}
