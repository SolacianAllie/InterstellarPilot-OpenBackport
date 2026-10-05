using System.Collections.Generic;
using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldPopulation
{
	public class WorldPopulatorSectorTypeSettings : MonoBehaviour
	{
		public int MinNumberPlanetSectors = 1;

		public int MinNumberAsteroidSectors = 1;

		public List<WorldPopulatorSectorType> SectorTypes = new List<WorldPopulatorSectorType>();
	}
}
