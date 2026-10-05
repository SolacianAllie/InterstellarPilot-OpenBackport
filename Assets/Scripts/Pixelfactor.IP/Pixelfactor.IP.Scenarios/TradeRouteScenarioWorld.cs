using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.IP.Engine.Triggers;

namespace Pixelfactor.IP.Scenarios
{
	public class TradeRouteScenarioWorld : WorldAdvanced
	{
		private int numBlackSailShipsDestroyed;

		public Faction PirateFaction;

		public int RequiredDestroyedShipCount = 5;

		public TriggerGroup ShipsKilledTrigger;

		public int NumBlackSailShipsDestroyed
		{
			get
			{
				return numBlackSailShipsDestroyed;
			}
			set
			{
				numBlackSailShipsDestroyed = value;
			}
		}

		protected override void OnNewGame()
		{
			base.OnNewGame();
			Engine.LocalFaction.Intel.DiscoverAllSectors();
		}

		protected override void OnUnitKilled(Unit unit, Faction attackerFaction)
		{
			base.OnUnitKilled(unit, attackerFaction);
			if (attackerFaction != null && attackerFaction.IsPlayerFaction && unit.Faction == PirateFaction)
			{
				numBlackSailShipsDestroyed++;
				if (numBlackSailShipsDestroyed == RequiredDestroyedShipCount)
				{
					ShipsKilledTrigger.gameObject.SetActive(value: true);
				}
			}
		}
	}
}
