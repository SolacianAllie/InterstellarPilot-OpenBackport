using Pixelfactor.IP.Common;
using Pixelfactor.IP.SavedGames.V2.Model.Scenarios;

namespace Pixelfactor.IP.SavedGames.V2.Model
{
	public class ModelScenarioData
	{
		public double NextRandomEventTime { get; set; }

		public bool HasRandomEvents { get; set; }

		public ModelFactionSpawner FactionSpawner { get; set; }

		public ModelTradeRouteScenarioData TradeRouteScenarioData { get; set; }

		public RespawnOnDeathPreference RespawnOnDeath { get; set; }

		public bool Permadeath { get; set; }

		public bool AllowTeleporting { get; set; } = true;

		public bool AsteroidRespawningEnabled { get; set; } = true;

		public float AsteroidRespawnTime { get; set; } = 0.5f;

		public float NextProcessOtherEventsTime { get; set; }

		public bool AllowStationCapture { get; set; } = true;

		public bool AllowAbandonShip { get; set; } = true;
	}
}
