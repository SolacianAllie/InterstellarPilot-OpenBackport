using Pixelfactor.IP.Engine.Factions;
using UnityEngine;

namespace Pixelfactor.IP.Engine.ActiveEffects
{
	public class RevealTargetActiveEffect : ActiveEffectBase
	{
		public override LayerMask? TargetMask => GameController.Instance.AIScanMask;

		public override void ApplyToUnit(Unit unit, Faction sourceFaction)
		{
			if (sourceFaction != null && unit.IsDiscoverableType)
			{
				sourceFaction.Intel.DiscoverUnit(unit);
			}
		}
	}
}
