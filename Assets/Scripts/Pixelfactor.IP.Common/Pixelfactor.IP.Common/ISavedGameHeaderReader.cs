using System.IO;

namespace Pixelfactor.IP.Common
{
	public interface ISavedGameHeaderReader
	{
		ISavedGameHeader Read(BinaryReader binaryReader);
	}
}
