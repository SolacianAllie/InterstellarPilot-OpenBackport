using System;
using Pixelfactor.IP.Engine.WorldSeeding;

namespace Pixelfactor.IP.Engine
{
	[Serializable]
	public class ScenarioLoadData
	{
		public string FullSaveGamePath;

		public ScenarioInfo ScenarioInfo;

		public Version SaveVersion;

		public WorldSeedSettings SeedSettings;

		public int CustomSeed = -1;
	}
}
