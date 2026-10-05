using Pixelfactor.IP.Engine.Factions;
using UnityEngine;

namespace Pixelfactor.IP.Engine.ActiveEffects
{
	public class ActiveEffectBase : MonoBehaviour
	{
		public string Name;

		public virtual LayerMask? TargetMask => null;

		public virtual void ApplyToUnit(Unit unit, Faction sourceFaction)
		{
		}
	}
}
