using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.UI.Screens.Orders.Buttons
{
	public class DockAtMyTargetOrderButton : DockOrderButton
	{
		protected override Unit GetTargetUnit()
		{
			return HudTarget;
		}
	}
}
