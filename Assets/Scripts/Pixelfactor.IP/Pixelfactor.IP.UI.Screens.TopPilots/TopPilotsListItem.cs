using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.IP.UI.Screens.Bounty;
using TMPro;

namespace Pixelfactor.IP.UI.Screens.TopPilots
{
	public class TopPilotsListItem : ScrollListItem<Person>
	{
		public TextMeshProUGUI KillsLabel;

		public TextMeshProUGUI NameLabel;

		public TextMeshProUGUI FactionLabel;

		public TextMeshProUGUI LastKnownLocationLabel;

		public override void Refresh()
		{
			base.Refresh();
			if (Item != null)
			{
				NameLabel.text = Item.GetNameAndTitleOrFullRank(shortName: false);
				FactionLabel.text = ((Item.Faction != null) ? Item.Faction.GetDescriptiveFactionName(shortName: false) : "-");
				FactionLabel.color = EngineASX.Instance.GetFactionHostilityColorForPlayerTarget(Item.Faction);
				KillsLabel.text = TextFormattingHelper.FormatNumber(Item.Kills);
				LastKnownLocationLabel.text = GetLastKnownLocationText(Item);
			}
		}

		private string GetLastKnownLocationText(Person item)
		{
			if (item.CurrentUnit != null)
			{
				FactionIntel.DiscoveredUnitData? discoveryData = EngineASX.Instance.LocalFaction.Intel.GetDiscoveryData(item.CurrentUnit);
				if (discoveryData.HasValue)
				{
					string lastKnownLocationText = BountyScreenItem.GetLastKnownLocationText(discoveryData.Value.Sector, discoveryData.Value.Unit, discoveryData.Value.TimeOfDiscovery);
					if (!string.IsNullOrWhiteSpace(lastKnownLocationText))
					{
						return lastKnownLocationText;
					}
				}
			}
			return "-";
		}
	}
}
