using System.IO;
using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.Engine.SaveGame;
using Pixelfactor.IP.Engine.UnitComponents;
using UnityEngine;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Unit_ChangeCargo : EngineAction
	{
		public CargoClass CargoClass;

		public bool IgnoreCapacity = true;

		public int Quantity = 1;

		public Unit TargetUnit;

		public override ActionType Type => ActionType.Unit_ChangeCargo;

		public override void Execute()
		{
			base.Execute();
			if (TargetUnit != null)
			{
				CargoBayComponent cargoBayComponent = TargetUnit.CargoBayComponent;
				if (cargoBayComponent != null)
				{
					cargoBayComponent.AddToCargo(CargoClass, Quantity, IgnoreCapacity);
				}
				else
				{
					Debug.LogWarning("Cannot change cargo. Null Cargo bay", this);
				}
			}
			else
			{
				Debug.LogWarning("Cannot change cargo. Null Unit", this);
			}
		}

		public override void WriteBinary(BinaryWriter writer, EngineASX engine)
		{
			base.WriteBinary(writer, engine);
			writer.WriteUnitId(TargetUnit);
		}

		public override void ReadBinary(BinaryReader reader, EngineASX engine)
		{
			base.ReadBinary(reader, engine);
			TargetUnit = reader.ReadUnitFromId(engine);
		}
	}
}
