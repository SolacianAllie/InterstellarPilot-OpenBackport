using Pixelfactor.IP.Engine.Fleets.FleetFormations;
using TMPro;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Screens.FleetFormations
{
	public class FleetFormationStyleItem : ScrollListItem<FleetFormationStyle>
	{
		public TextMeshProUGUI Label;

		public Image FormationImage;

		public override void Refresh()
		{
			base.Refresh();
			Label.text = Item.Name;
			FormationImage.sprite = Item.Sprite;
		}
	}
}
