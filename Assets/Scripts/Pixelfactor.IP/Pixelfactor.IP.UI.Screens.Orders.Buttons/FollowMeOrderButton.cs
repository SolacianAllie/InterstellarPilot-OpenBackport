using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP.UI.Screens.Orders.Buttons
{
	public class FollowMeOrderButton : FollowOrderButton
	{
		protected override Unit GetTargetUnit()
		{
			return EngineASX.Instance.PlayerUnit;
		}
	}
}
