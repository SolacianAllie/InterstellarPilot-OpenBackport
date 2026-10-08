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

		// Open Frontier: which of the 6 shield sections was hit - the
		// effect reads that section's LIVE UI health color every frame.
		public int ShieldIndex;

		// True when the hit emptied the section: the effect pops bright
		// red and dies fast instead of playing the full fade.
		public bool DepletedFlash;

		// True only for a freshly spawned effect: the visual grows from
		// 0 to full size over its first 0.5s. Envelope resets (repeat
		// hits while the visual lives) clear this - no re-scaling until
		// the shield has fully faded away.
		public bool ScaleIn;

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
