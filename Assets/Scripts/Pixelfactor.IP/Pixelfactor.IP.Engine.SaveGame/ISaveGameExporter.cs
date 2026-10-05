using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.Engine.SaveGame
{
	public interface ISaveGameExporter
	{
		ISavedGame Export(EngineASX engine);
	}
}
