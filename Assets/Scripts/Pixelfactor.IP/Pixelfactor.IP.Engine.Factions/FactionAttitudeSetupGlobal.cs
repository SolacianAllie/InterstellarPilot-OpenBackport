using System;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Factions
{
	[Serializable]
	public class FactionAttitudeSetupGlobal : FactionAttitudeSetup
	{
		protected override void apply(EngineASX engine, Faction sourceFaction, bool forceTwoWay)
		{
			if (sourceFaction != null)
			{
				foreach (Faction faction in engine.Factions)
				{
					if (faction.UniqueId != sourceFaction.UniqueId)
					{
						ApplyToFaction(engine, sourceFaction, faction, forceTwoWay);
					}
				}
				return;
			}
			Debug.LogWarning("Cannot applying global faction attitude. Source faction is null. Source FactionClass: " + sourceFaction);
		}
	}
}
