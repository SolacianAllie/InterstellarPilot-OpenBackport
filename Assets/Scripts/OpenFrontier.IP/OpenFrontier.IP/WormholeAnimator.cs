using UnityEngine;

namespace OpenFrontier.IP
{
	public class WormholeAnimator : MonoBehaviour
	{
		public delegate void AnimationFinishedHandler(WormholeAnimator sender);

		public AudioSource EntryAudioSourcePrefab;

		public AudioSource FinishAudioSourcePrefab;

		public MoveGameObject Mover;

		public event AnimationFinishedHandler AnimationFinished;

		// TEMP diagnostics for the black-transition hunt - remove after fixing.
		private float spawnTime;

		private void Awake()
		{
			spawnTime = Time.realtimeSinceStartup;
			Object.Instantiate(EntryAudioSourcePrefab);
			Debug.Log($"[WormholeAnim] spawned t={spawnTime:0.00} renderers={GetComponentsInChildren<Renderer>().Length} moverEnabled={Mover != null && Mover.enabled}");
		}

		private void Update()
		{
			if (!Mover.enabled)
			{
				Debug.Log($"[WormholeAnim] finishing after {Time.realtimeSinceStartup - spawnTime:0.00}s moverPos={Mover.transform.position}");
				if (AnimationFinished != null)
				{
					AnimationFinished(this);
				}
				if (FinishAudioSourcePrefab != null)
				{
					Object.Instantiate(FinishAudioSourcePrefab.gameObject);
				}
				enabled = false;
			}
		}
	}
}
