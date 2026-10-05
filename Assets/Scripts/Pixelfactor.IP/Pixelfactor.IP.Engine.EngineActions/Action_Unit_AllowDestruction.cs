using System.IO;
using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.Engine.SaveGame;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Unit_AllowDestruction : EngineAction
	{
		public bool AllowDestruction = true;

		public Unit Unit;

		public override ActionType Type => ActionType.Unit_AllowDestruction;

		public override void Execute()
		{
			base.Execute();
			if (Unit != null && Unit.Destructable != null)
			{
				Unit.Destructable.AllowDestruction = AllowDestruction;
			}
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
