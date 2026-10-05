using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.UI.Screens.Orders.Buttons
{
	public class DockAtMyWaypointOrderButton : DockOrderButton
	{
		protected override Unit GetTargetUnit()
		{
			return WaypointTargetUnit;
		}
	}
}
