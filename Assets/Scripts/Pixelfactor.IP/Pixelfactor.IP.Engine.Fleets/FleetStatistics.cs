using UnityEngine;

namespace Pixelfactor.IP.Engine.Fleets
{
	public class FleetStatistics : MonoBehaviour
	{
		public int NumTimesSentForRearming;

		public int NumTimesRegrouped;

		public int NumOrdersQueued;

		public int NumOrdersTimedOut;

		public int NumOrdersCompleted;

		public int MostShipsInFleet;

		public int NumTimesEnteredCombatInterception;
	}
}
