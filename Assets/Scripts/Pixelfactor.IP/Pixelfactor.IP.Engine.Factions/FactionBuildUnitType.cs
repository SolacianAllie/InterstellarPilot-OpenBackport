using System;
using Pixelfactor.Unity.Utils;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Factions
{
	[Serializable]
	public class FactionBuildUnitType : IWeighted
	{
		public UnitClass UnitClass;

		[SerializeField]
		private float weight;

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
