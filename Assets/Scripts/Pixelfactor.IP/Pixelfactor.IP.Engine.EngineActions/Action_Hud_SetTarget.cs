using System.IO;
using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.Engine.SaveGame;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Hud_SetTarget : EngineAction
	{
		public Unit Unit;

		public override ActionType Type => ActionType.Hud_SetTarget;

		public override void Execute()
		{
			base.Execute();
			engine.Hud.CurrentTarget = Unit;
		}

		public override void WriteBinary(BinaryWriter writer, EngineASX engine)
		{
			base.WriteBinary(writer, engine);
			writer.WriteUnitId(Unit);
		}

		public override void ReadBinary(BinaryReader reader, EngineASX engine)
		{
			base.ReadBinary(reader, engine);
			Unit = reader.ReadUnitFromId(engine);
		}
	}
}
