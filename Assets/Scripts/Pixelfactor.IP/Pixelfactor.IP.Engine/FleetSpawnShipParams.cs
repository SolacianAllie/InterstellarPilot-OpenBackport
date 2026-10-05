using System;
using UnityEngine.Serialization;

namespace Pixelfactor.IP.Engine
{
	[Serializable]
	public class FleetSpawnShipParams
	{
		[FormerlySerializedAs("PilotProfile")]
		public Person PilotPrefab;

		public string ShipName;

		public UnitClass UnitClass;

		public bool AddCargoLoadout = true;
	}
}
