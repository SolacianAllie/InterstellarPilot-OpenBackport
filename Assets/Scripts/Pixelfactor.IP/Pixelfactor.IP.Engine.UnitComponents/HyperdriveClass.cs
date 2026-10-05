using UnityEngine;

namespace Pixelfactor.IP.Engine.UnitComponents
{
	public class HyperdriveClass : TurretClass
	{
		public float ChargeTime = 8f;

		protected override ComponentBase createComponent(GameObject target)
		{
			HyperdriveComponent hyperdriveComponent = target.AddComponent<HyperdriveComponent>();
			hyperdriveComponent.HyperdriveClass = this;
			hyperdriveComponent.TurretClass = this;
			return hyperdriveComponent;
		}
	}
}
