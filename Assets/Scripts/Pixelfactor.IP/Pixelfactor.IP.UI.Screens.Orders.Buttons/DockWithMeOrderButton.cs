using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.UI.Screens.Orders.Buttons
{
	public class DockWithMeOrderButton : DockOrderButton
	{
		protected override Unit GetTargetUnit()
		{
			return EngineASX.Instance.PlayerUnit;
		}
	}
}
