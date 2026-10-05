using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Hud_ClearSpeech : EngineAction
	{
		public override ActionType Type => ActionType.Hud_ClearSpeech;

		public override void Execute()
		{
			base.Execute();
			engine.Hud.SpeechModel.ClearRequests();
		}
	}
}
