using Pixelfactor.IP.Engine;
using UnityEngine;

namespace Pixelfactor.IP
{
	public class ShieldHitInfo : MonoBehaviour
	{
		public Quaternion LocalRotation = Quaternion.identity;

		public Vector3 LocalTranslation = Vector3.zero;

		public float StartExpiryTime;

		public float StartTime;

		public Unit TargetUnit;

		public MeshRenderer Renderer;

		public float MaxScale;

		public void SetShieldHitOrientation(Vector3 damageSourceWorldPosition)
		{
			if (TargetUnit != null)
			{
				LocalTranslation = TargetUnit.transform.InverseTransformPoint(damageSourceWorldPosition);
				LocalRotation = Quaternion.LookRotation(LocalTranslation);
			}
		}
	}
}
