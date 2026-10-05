using System.Collections.Generic;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Settings
{
	public class HypersleepSettings : MonoBehaviour
	{
		public float DefaultTimeMultiplier = 4f;

		public float TimeMultiplierChangeRate = 1f;

		public List<HypersleepTimescaleSetting> TimescaleSettings;
	}
}
