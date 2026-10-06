using OpenFrontier.IP.Common.Factions;
using OpenFrontier.IP.Engine;
using OpenFrontier.IP.Engine.Factions;
using OpenFrontier.IP.UI.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OpenFrontier.IP.UI
{
	public class FactionListItemNew : ScrollListItem<Faction>
	{
		public Graphic HostileGraphic;

		public Text NeutralityLabel;

		public TextMeshProUGUI NameLabel;

		public Image OpinionBarBackgroundImage;

		public Image OpinionBarForegroundImage;

		public GameObject OpinionBarRoot;

		private float oldOpinion = -1f;

		public FactionsScreen ParentUI => ((FactionList)ParentList).DiplomacyUI;

		public override void Refresh()
		{
			base.Refresh();
			if (Item != null)
			{
				RefreshNameText();
				RefreshNeutralityLabel();
				bool flag = !Item.IsPlayerFaction;
				OpinionBarRoot.SetActive(flag);
				if (flag)
				{
					RefreshOpinionBar();
					RefreshOpinionBarColor();
				}
			}
		}

		private void Update()
		{
			if (Item != null)
			{
				RefreshOpinionBarColor();
				RefreshNeutralityLabel();
				float opinionOfLocalFaction = GetOpinionOfLocalFaction();
				if (opinionOfLocalFaction != oldOpinion)
				{
					RefreshOpinionBar();
					oldOpinion = opinionOfLocalFaction;
				}
			}
		}

		private void RefreshNameText()
		{
			NameLabel.text = Item.GetDescriptiveFactionNameIncludingPilotName(Item.IsFreelancer);
			NameLabel.color = (Item.IsPlayerFaction ? Item.Engine.OwnedColor : Item.Engine.AttitudeNeutralColor);
		}

		private void RefreshOpinionBar()
		{
			float num = Faction.ClampOpinion(GetOpinionOfLocalFaction());
			float newSize = CalculateOpinionBarWidth(num);
			OpinionBarForegroundImage.rectTransform.SetWidth(newSize);
			Vector2 pivot = OpinionBarForegroundImage.rectTransform.pivot;
			float x = ((num >= 0f) ? 0f : 1f);
			OpinionBarForegroundImage.rectTransform.pivot = new Vector2(x, pivot.y);
		}

		private float GetOpinionOfLocalFaction()
		{
			return Item.GetOpinion(ParentUI.Eng.LocalPlayer.Faction);
		}

		private float CalculateOpinionBarWidth(float opinion)
		{
			float num = OpinionBarBackgroundImage.rectTransform.rect.width / 2f;
			float num2 = Mathf.Abs(opinion) * num;
			if (num2 < ParentUI.MinOpinionBarScale)
			{
				num2 = ParentUI.MinOpinionBarScale;
			}
			return num2;
		}

		public void RefreshOpinionBarColor()
		{
			OpinionBarForegroundImage.color = ParentUI.Eng.GetFactionHostilityColor(Item, ParentUI.Eng.LocalPlayer.Faction);
		}

		public void RefreshNeutralityLabel()
		{
			Neutrality neutralityWith = Item.GetNeutralityWith(ParentUI.Eng.LocalPlayer.Faction);
			NeutralityLabel.gameObject.SetActive(neutralityWith != Neutrality.Neutral);
			HostileGraphic.enabled = neutralityWith == Neutrality.Hostile;
			switch (neutralityWith)
			{
			case Neutrality.Allied:
				NeutralityLabel.text = "Allied";
				NeutralityLabel.color = EngineASX.Instance.AlliedColor;
				break;
			default:
				NeutralityLabel.text = "Hostile";
				NeutralityLabel.color = EngineASX.Instance.HostilityColor;
				break;
			case Neutrality.Neutral:
				break;
			}
		}
	}
}
