using System.IO;
using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.IP.Engine.SaveGame;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_Faction_SetRpMultiplier : EngineAction
	{
		public Faction Faction;

		public float RpMultiplier = 1f;

		public override ActionType Type => ActionType.Faction_SetRpMultiplier;

		public override void Execute()
		{
			base.Execute();
			if (Faction != null)
			{
				Faction.RequisitionPointMultiplier = RpMultiplier;
			}
		}

		public override void WriteBinary(BinaryWriter writer, EngineASX engine)
		{
			base.WriteBinary(writer, engine);
			writer.WriteFactionId(Faction);
		}

		public override void ReadBinary(BinaryReader reader, EngineASX engine)
		{
			base.ReadBinary(reader, engine);
			Faction = reader.ReadFactionFromId(engine);
		}
	}
}
