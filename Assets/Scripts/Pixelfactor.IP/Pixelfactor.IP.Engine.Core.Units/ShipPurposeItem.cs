using System;
using Pixelfactor.IP.Common.Factions;

namespace Pixelfactor.IP.Engine.Core.Units
{
	[Serializable]
	public class ShipPurposeItem
	{
		public FactionStrategy FactionStrategy;

		public float Effectiveness = 1f;
	}
}
