using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.Engine.SaveGame
{
	public interface ISaveGameImporter
	{
		void Import(ISavedGame savedGame, EngineASX engine);
	}
}
