using OpenFrontier.IP.Engine;
using UnityEngine;

namespace OpenFrontier.IP
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

		// Open Frontier: the shield section's health color SNAPSHOT at
		// the moment of impact (the same GetUnitShieldColor gradient
		// the UI widgets use).
		public Color HitColor = Color.white;

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
