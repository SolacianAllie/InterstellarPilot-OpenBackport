using UnityEngine;

namespace Pixelfactor.IP.Scratchcard.Model
{
	public class ScratchcardPrize : MonoBehaviour
	{
		public int PrizeValue;

		public int TicketCount;

		public bool IsWinner => PrizeValue > 0;
	}
}
