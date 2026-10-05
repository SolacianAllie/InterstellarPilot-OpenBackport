using System;
using UnityEngine;

namespace Pixelfactor.IP.Hints
{
	public class HintInfo : MonoBehaviour
	{
		public int UniqueId = -1;

		public DateTime? LastShown;

		public int TimesShown;
	}
}
