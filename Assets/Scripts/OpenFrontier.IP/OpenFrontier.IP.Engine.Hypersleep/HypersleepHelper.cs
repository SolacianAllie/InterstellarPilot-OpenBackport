using OpenFrontier.IP.UI;
using OpenFrontier.IP.UI.Screens;
using OpenFrontier.IP.UI.Screens.Hypersleep;
using OpenFrontier.IP.UI.Screens.MessageBox;

namespace OpenFrontier.IP.Engine.Hypersleep
{
	public static class HypersleepHelper
	{
		private static object MessageBox;

		public static bool ShouldShowHypersleepOption()
		{
			return EngineASX.Instance.World.Permissions.AllowHypersleep;
		}

		public static bool IsHypersleepAvailable(out string reason)
		{
			reason = null;
			Unit localRootUnit = EngineASX.Instance.LocalRootUnit;
			if (localRootUnit.IsUnderAttack())
			{
				reason = "Cannot enter hypersleep when current " + (localRootUnit.IsShip() ? "ship" : "station") + " is under attack";
				return false;
			}
			return true;
		}

		public static void TryEnterHypersleep()
		{
			if (!IsHypersleepAvailable(out var reason))
			{
				UIController.Instance.ShowMessageBox(reason, MessageBoxButtons.Ok, null, MessageBoxIcon.Warning);
			}
			else
			{
				UIController.Instance.ScreenNavigator.ShowHypersleepScreen();
			}
		}

		public static bool IsHypersleepActive()
		{
			return UIController.Instance.ScreenNavigator.CurrentScreen is HypersleepScreen;
		}
	}
}
