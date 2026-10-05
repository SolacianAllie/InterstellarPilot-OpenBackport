using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Scratchcard.Model;
using Pixelfactor.IP.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.Scratchcard.UI
{
	public class ScratchcardPrizeItem : MonoBehaviour
	{
		public Text Label;

		public SpriteSineColor ActivatedGameObject;

		private ScratchcardPrize prize;

		public Image Image;

		public ScratchcardPrize Prize
		{
			get
			{
				return prize;
			}
			set
			{
				prize = value;
			}
		}

		public void Refresh()
		{
			ActivatedGameObject.Reset();
			ActivatedGameObject.enabled = false;
			Label.text = TextFormattingHelper.FormatCredits(prize.PrizeValue);
		}

		public void HighlightWin()
		{
			ActivatedGameObject.Reset();
			ActivatedGameObject.enabled = true;
		}
	}
}
