using UnityEngine;

namespace OpenFrontier.IP.Engine
{
	/// <summary>
	/// Open Frontier: re-rolls the main menu's star system on EVERY menu
	/// visit so the backdrop is a fresh randomly generated system each
	/// time, custom-universe style. Detects the menu scenario the same way
	/// EngineMusicPlayerController does (World.ScenarioInfo matches
	/// GameController.MainMenuScenario), then:
	///   1. re-seeds the sector and regenerates its backdrop (nebulae +
	///      starfield params re-rolled via the stock pipeline)
	///   2. rolls a new star color via the seeded HSV generator and writes
	///      it into the sector's DirectionLightColor (drives light, sun
	///      billboard, tint and the reflection probe's anchor sun)
	///   3. jitters the sky exposure
	/// ActiveSectorData.OnSectorChanged applies all of it (backdrop regen
	/// + sun tint refresh + probe recapture).
	/// Population themes (calm / mining / bandits) build on top of this.
	/// Lives on the persistent GameController.
	/// </summary>
	public class MainMenuWorldController : MonoBehaviour
	{
		private ScenarioInfo appliedScenario;

		private void Update()
		{
			if (!EngineASX.LoadedAndReady || EngineASX.Instance.World == null || GameController.Instance == null)
			{
				return;
			}
			ScenarioInfo scenarioInfo = EngineASX.Instance.World.ScenarioInfo;
			if (scenarioInfo != appliedScenario)
			{
				appliedScenario = scenarioInfo;
				if (IsMainMenuScenario(scenarioInfo))
				{
					RollMenuSystem();
				}
			}
		}

		private static bool IsMainMenuScenario(ScenarioInfo scenarioInfo)
		{
			ScenarioInfo mainMenuScenario = GameController.Instance.MainMenuScenario;
			return scenarioInfo != null && mainMenuScenario != null && (scenarioInfo == mainMenuScenario || scenarioInfo.UniqueId == mainMenuScenario.UniqueId);
		}

		private static void RollMenuSystem()
		{
			Sector sector = EngineASX.Instance.ActiveSector;
			if (sector == null || EngineASX.Instance.ActiveSectorData == null)
			{
				return;
			}
			int num = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
			System.Random random = new System.Random(num);
			// Fresh backdrop (nebulae/starfield re-rolled from the new seed)
			// and a fresh star color from the seeded generator.
			sector.RandomSeed = num;
			sector.DirectionLightColor = StarColorGenerator.ForSector(num);
			sector.SkyExposure = Mathf.Lerp(0.7f, 1.3f, (float)random.NextDouble());
			// Applies everything: regenerates the backdrop, refreshes the
			// sun tint, recaptures the reflection probe.
			EngineASX.Instance.ActiveSectorData.OnSectorChanged();
			Debug.Log("[MainMenuWorldController] rolled menu system, seed " + num);
		}
	}
}
