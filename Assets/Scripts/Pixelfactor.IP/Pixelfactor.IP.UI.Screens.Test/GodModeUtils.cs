using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.UI.Screens.Test
{
	public static class GodModeUtils
	{
		public static void OnGodModeActionExecuted()
		{
			EngineASX.Instance.World.HasCheated = true;
		}
	}
}
