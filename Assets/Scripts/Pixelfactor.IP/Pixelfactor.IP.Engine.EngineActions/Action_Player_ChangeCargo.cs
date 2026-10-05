using Pixelfactor.IP.Common.Triggers;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Player_ChangeCargo : EngineAction
	{
		public CargoClass CargoClass;

		public bool IgnoreCapacity = true;

		public int Quantity = 1;

		public override ActionType Type => ActionType.Player_ChangeCargo;

		public override void Execute()
		{
			base.Execute();
			engine.TransferToPlayerCargoWithMsg(engine.LocalPlayer.Person.CurrentUnit, CargoClass, Quantity, IgnoreCapacity);
		}
	}
}
