using System;
using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.Engine
{
	[Serializable]
	public class DamageType
	{
		public float Damage;

		public float MiningDamage;

		public ShieldDamageType ShieldDamageType;

		public float ComponentDamageMultiplier = 1f;
	}
}
