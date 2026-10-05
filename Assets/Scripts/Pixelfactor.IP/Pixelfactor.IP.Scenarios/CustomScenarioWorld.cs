using System;
using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.Scenarios
{
	public class CustomScenarioWorld : WorldBase
	{
		protected override void OnNewGame()
		{
			throw new Exception("Not supported");
		}

		protected override void OnLoadGame()
		{
			base.OnLoadGame();
			SeedWorldOnNewGame();
			CanLoadLastSave = true;
		}
	}
}
