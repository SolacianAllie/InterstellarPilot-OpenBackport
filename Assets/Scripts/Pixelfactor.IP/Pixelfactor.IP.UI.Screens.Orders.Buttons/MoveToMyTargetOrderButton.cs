using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.UI.Screens.Orders.Buttons
{
	public class MoveToMyTargetOrderButton : MoveToUnitOrderButton
	{
		protected override Unit GetTargetUnit()
		{
			return HudTarget;
		}
	}
}
