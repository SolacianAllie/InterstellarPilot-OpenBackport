using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.AI.ActiveOrders;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Fleets.ActiveObjectives
{
	public class ActiveUndockOrder : ActiveFleetOrder
	{
		private float nextCheckTime;

		protected override void tick(float elapsedTime)
		{
			base.tick(elapsedTime);
			if (Time.time > nextCheckTime)
			{
				nextCheckTime = Time.time + 2f;
				if (fleet.AreAllShipsUndocked())
				{
					OnComplete();
				}
			}
		}

		public override DockedPreference GetPreferToDockWhenIdle()
		{
			return DockedPreference.Undock;
		}
	}
}
