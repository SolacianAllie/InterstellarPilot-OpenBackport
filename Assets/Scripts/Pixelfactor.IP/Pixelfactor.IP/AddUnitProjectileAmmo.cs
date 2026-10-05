using Pixelfactor.IP.Engine;
using UnityEngine;

namespace Pixelfactor.IP
{
	[RequireComponent(typeof(UnitComponentHolder))]
	public class AddUnitProjectileAmmo : MonoBehaviour
	{
		public bool AutoApply;

		public float CountMultiplier = 1f;

		public bool IgnoreBayCapacity = true;

		private UnitComponentHolder unitComponents;

		private void Awake()
		{
			unitComponents = GetComponent<UnitComponentHolder>();
		}

		private void Update()
		{
			if (AutoApply && unitComponents != null && unitComponents.HasInit)
			{
				unitComponents.AddDefaultProjectileAmmo(CountMultiplier, IgnoreBayCapacity);
				Object.Destroy(this);
			}
		}
	}
}
