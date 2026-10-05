using System.IO;
using Pixelfactor.IP.Common.Triggers;
using UnityEngine;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class EngineAction : MonoBehaviour
	{
		public virtual ActionType Type => ActionType.Unspecified;

		protected EngineASX engine => EngineASX.Instance;

		public void Validate()
		{
			validate();
		}

		protected virtual void validate()
		{
		}

		public virtual void Execute()
		{
		}

		public virtual void WriteBinary(BinaryWriter writer, EngineASX engine)
		{
		}

		public virtual void ReadBinary(BinaryReader reader, EngineASX engine)
		{
		}
	}
}
