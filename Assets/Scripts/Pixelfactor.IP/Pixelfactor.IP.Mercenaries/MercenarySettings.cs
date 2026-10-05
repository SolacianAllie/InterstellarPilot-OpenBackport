using System.Collections.Generic;
using UnityEngine;

namespace Pixelfactor.IP.Mercenaries
{
	public class MercenarySettings : MonoBehaviour
	{
		public List<MercenaryHireDurationSetting> HireDurationSettings;

		public float CombatRatingToHourRateConversion = 2000f;
	}
}
