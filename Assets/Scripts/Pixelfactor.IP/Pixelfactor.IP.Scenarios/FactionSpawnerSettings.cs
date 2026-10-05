using Pixelfactor.IP.Engine;
using UnityEngine;

namespace Pixelfactor.IP.Scenarios
{
	public class FactionSpawnerSettings : MonoBehaviour
	{
		public UnitClass RefineryUnitClass;

		public float TimeBetweenFactionSpawns = 600f;

		public float TimeBetweenMinorFactionSpawns = 60f;

		public FactionSpawnerSpawnType[] FactionTypes;
	}
}
