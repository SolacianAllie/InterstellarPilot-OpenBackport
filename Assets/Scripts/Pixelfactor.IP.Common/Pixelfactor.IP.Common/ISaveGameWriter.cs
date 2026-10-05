using System;
using System.IO;

namespace Pixelfactor.IP.Common
{
	public interface ISaveGameWriter
	{
		void Write(BinaryWriter writer, ISavedGame savedGame, Action<string> logger);
	}
}
