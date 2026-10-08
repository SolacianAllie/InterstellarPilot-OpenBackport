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
		// the UI widgets use), and which of the 6 sections was hit (for
		// the live color the effect fades toward).
		public Color HitColor = Color.white;

		public int ShieldIndex;

		// True when the hit emptied the section: the effect pops bright
		// white and dies fast instead of playing the full fade.
		public bool DepletedFlash;

		// Effect lifetime in seconds (short for DepletedFlash, else the
		// renderer's ShieldHitDuration).
		public float Duration;

		// Per-pool-object instanced material (created once from
		// Renderer.material) - the tint goes here.
		public Material CachedMaterial;

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
