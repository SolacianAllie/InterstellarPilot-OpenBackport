using System;
using Pixelfactor.IP.Engine.Fleets.FleetFormations;
using Pixelfactor.Unity.Utils;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Settings
{
	[Serializable]
	public class NpcFleetFormationStylePreference : IWeighted
	{
		[SerializeField]
		private float weight = 1f;

		public FleetFormationStyle FleetFormationStyle;

		public float Weight
		{
			get
			{
				return weight;
			}
			set
			{
				weight = value;
			}
		}
	}
}
