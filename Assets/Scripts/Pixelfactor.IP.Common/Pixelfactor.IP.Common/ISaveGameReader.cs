using System.IO;

namespace Pixelfactor.IP.Common
{
	public interface ISaveGameReader
	{
		ISavedGame Read(BinaryReader reader);
	}
}
