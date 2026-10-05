using Pixelfactor.IP.Engine;
using Pixelfactor.IP.UI.Screens.Skirmish;
using Pixelfactor.IP.UI.Screens.UniverseSandboxSettings;
using Pixelfactor.IP.UI.Screens.UniverseSelect;

namespace Pixelfactor.IP.UI.Screens.GameMode
{
	public class GameModeScreen : ScreenBase
	{
		public void NavigateToUniverse()
		{
			UIController.Instance.ScreenNavigator.NavigateToScreen<UniverseSelectScreen>();
		}

		public void NavigateToUniverseSandbox()
		{
			UIController.Instance.ScreenNavigator.NavigateToScreen<UniverseSandboxSettingsScreen>();
		}

		public void NavigateToScenarios()
		{
			UIController.Instance.ScreenNavigator.ShowScenariosScreen(GameController.EngineLaunchSource.ScenariosUI, showTutorials: false, showMissions: true);
		}

		public void NavigateToSkirmish()
		{
			UIController.Instance.ScreenNavigator.NavigateToScreen<SkirmishSetupScreen>();
		}

		public void NavigateToTutorials()
		{
			UIController.Instance.ScreenNavigator.ShowTutorialsScreen();
		}

		public void NavigateToBattles()
		{
			UIController.Instance.ScreenNavigator.ShowBattlesScreen();
		}
	}
}
