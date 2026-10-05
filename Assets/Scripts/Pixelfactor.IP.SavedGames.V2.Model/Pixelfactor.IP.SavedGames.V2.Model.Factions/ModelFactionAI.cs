using System.Collections.Generic;
using Pixelfactor.IP.Common.Factions;

namespace Pixelfactor.IP.SavedGames.V2.Model.Factions
{
	public class ModelFactionAI
	{
		public double NextUnitSpawnTime { get; set; }

		public int NumFleetsSpawned { get; set; }

		public int NumUnitsSpawned { get; set; }

		public bool SpawnOnlyAtOwnedDocks { get; set; }

		public double LastBuiltUnitTime { get; set; }

		public double LastOrderedPatrolTime { get; set; }

		public FactionSpawnMode SpawnMode { get; set; }

		public List<ModelSector> SpawnSectors { get; set; } = new List<ModelSector>();

		public List<ModelUnit> ExcludedUnits { get; set; } = new List<ModelUnit>();

		public ModelFactionMercenaryHireInfo FactionMercenaryHireInfo { get; set; }

		public virtual FactionAIType AIType => FactionAIType.Generic;
	}
}
