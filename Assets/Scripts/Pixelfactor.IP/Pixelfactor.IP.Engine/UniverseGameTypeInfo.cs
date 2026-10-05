using System.Collections.Generic;
using Pixelfactor.IP.Common;
using Pixelfactor.IP.billing;
using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public class UniverseGameTypeInfo : MonoBehaviour
	{
		public string Name;

		public UnitClass DisplayIconOverrideUnitClass;

		public float DefaultVirtue = 0.5f;

		public float SpawnLocationVirtuePreference = 0.5f;

		[TextArea(3, 6)]
		public string Description;

		public UniverseGameType GameType;

		public UniverseGameTypeSpawnUnit PlayerSpawnUnit;

		public List<UniverseGameTypeSpawnUnit> SpawnUnits = new List<UniverseGameTypeSpawnUnit>();

		public int StartingCredits = 10000;

		public List<IPProduct> RequiredProducts = new List<IPProduct>();

		public SecurityTypePreference SecurityTypePreference;

		public float SecurityTypePreferenceMultiplier = 0.2f;

		public float SpawnLocationVirtuePreferenceMultiplier = 1f;

		public float MinBonusWithSpawnedStationFaction = 0.2f;

		public float MaxBonusWithSpawnedStationFaction = 0.4f;
	}
}
