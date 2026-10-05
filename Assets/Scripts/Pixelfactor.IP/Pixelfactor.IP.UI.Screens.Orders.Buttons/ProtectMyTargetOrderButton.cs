using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.UI.Screens.Orders.Buttons
{
	public class ProtectMyTargetOrderButton : ProtectOrderButton
	{
		protected override Unit GetTargetUnit()
		{
			return HudTarget;
		}
	}
}
