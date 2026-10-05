using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Player_NewMessageSimple : EngineAction
	{
		public bool Notifications = true;

		public string From;

		public string Message;

		public string Subject;

		public string To = "#player#";

		public override ActionType Type => ActionType.Player_NewMessageSimple;

		public override void Execute()
		{
			base.Execute();
			engine.LocalPlayer.AddMessage(From, To, Subject, Message, Notifications);
		}
	}
}
