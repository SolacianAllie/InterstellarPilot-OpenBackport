using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OpenFrontier
{
	/// <summary>
	/// OPEN BACKPORT (Unity 6.3 / GLES 3.0): post-processing is disabled
	/// outright, at every quality tier.
	///
	/// On the hardware this branch targets - Adreno 308 and similar, 2 GB
	/// RAM - the bloom pyramid plus URP's final grading pass cost far more
	/// than the look is worth, and testers reported unplayable choppiness.
	///
	/// The flag has to be forced down from three places, because each one
	/// re-enables it:
	///   * UrpCameraStacker sets it on the camera-stack base camera
	///   * GameCamera.prefab has it authored on
	///   * UI screens spawn cameras at runtime, after this component runs
	/// so this hooks Camera.onPreCull and sweeps every camera rather than
	/// patching a known list.
	///
	/// On Open Frontier this file drives the bloom quality ladder instead
	/// (it is the same component, re-implemented for that branch).
	/// </summary>
	public class PostProcessingQualitySync : MonoBehaviour
	{
		private void Start()
		{
			// Subscribed exactly once, here. Doing it from the quality-change
			// path instead would stack a duplicate delegate every time the
			// player moved the quality slider.
			Camera.onPreCull += ApplyTo;
		}

		private void OnDestroy()
		{
			Camera.onPreCull -= ApplyTo;
		}

		private static void ApplyTo(Camera cam)
		{
			if (cam == null)
			{
				return;
			}

			var data = cam.GetUniversalAdditionalCameraData();
			if (data.renderPostProcessing)
			{
				data.renderPostProcessing = false;
			}
		}
	}
}
