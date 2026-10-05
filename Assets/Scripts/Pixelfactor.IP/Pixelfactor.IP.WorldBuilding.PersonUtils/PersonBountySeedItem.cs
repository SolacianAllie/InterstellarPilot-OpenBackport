using System;
using Pixelfactor.IP.Engine.Factions;

namespace Pixelfactor.IP.WorldBuilding.PersonUtils
{
	[Serializable]
	public class PersonBountySeedItem
	{
		public Faction FactionPlacingBounty;

		public int Bounty;

		public Faction BountyBoardFaction;

		public float ProbabilityOfUpdatingLastKnownPosition = 1f;
	}
}
