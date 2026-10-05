using System.IO;
using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.Engine.SaveGame;
using UnityEngine.Serialization;

namespace Pixelfactor.IP.Engine.Triggers
{
	public class Trigger_Fleet_HostileTargets : TriggerBase
	{
		[FormerlySerializedAs("Group")]
		public Fleet Fleet;

		public override TriggerType Type => TriggerType.Fleet_HostileTargets;

		public override void WriteBinary(BinaryWriter writer, EngineASX engine)
		{
			base.WriteBinary(writer, engine);
			writer.WriteAIGroupId(Fleet);
		}

		public override void ReadBinary(BinaryReader reader, EngineASX engine)
		{
			base.ReadBinary(reader, engine);
			Fleet = reader.ReadFleetFromId(engine);
		}

		protected override bool evaluate(EngineASX engine)
		{
			if (Fleet != null)
			{
				return Fleet.HasHostileTargets;
			}
			return false;
		}
	}
}
