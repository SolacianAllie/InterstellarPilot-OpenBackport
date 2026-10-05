using Pixelfactor.IP.UI.Screens.Test.MoveCurrentUnit;
using Pixelfactor.IP.UI.Screens.Test.Timescale;

namespace Pixelfactor.IP.UI.Screens.Test
{
	public class GodModeScreen : ScreenBase
	{
		public MoveOptionsGenerator MoveCurrentUnitOptionsGenerator;

		public TimescaleOptionsGenerator TimescaleOptionsGenerator;

		protected override void awake()
		{
			base.awake();
			if (MoveCurrentUnitOptionsGenerator != null)
			{
				MoveCurrentUnitOptionsGenerator.Generate();
			}
			if (TimescaleOptionsGenerator != null)
			{
				TimescaleOptionsGenerator.Generate();
			}
		}
	}
}
