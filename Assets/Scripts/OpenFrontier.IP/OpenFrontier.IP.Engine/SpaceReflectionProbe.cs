using UnityEngine;
using UnityEngine.Rendering;

namespace OpenFrontier.IP.Engine
{
	/// <summary>
	/// Open Frontier: realtime reflection probe that captures only the space
	/// backdrop layers (DeepSpace, BackgroundPlanet, Wormhole), so ships and
	/// nearby objects reflect the starfield, sun and distant bodies instead
	/// of each other or nothing. Clears to opaque black (not the skybox) so
	/// empty space reflects as black rather than any odd skybox colors.
	/// Follows the world camera and re-renders via scripting once per second
	/// (cheap: small resolution, sparse layers).
	/// Spawned by ActiveSectorData under the EngineASX root.
	/// </summary>
	public class SpaceReflectionProbe : MonoBehaviour
	{
		private const float SecondsBetweenRenders = 1f;

		private const int ProbeResolution = 128;

		// DeepSpace (30) | BackgroundPlanet (26) | Wormhole (22)
		private const int CullingMask = 1073741824 | 67108864 | 4194304;

		private ReflectionProbe probe;

		private float nextRenderTime;

		private void Awake()
		{
			probe = gameObject.AddComponent<ReflectionProbe>();
			probe.mode = ReflectionProbeMode.Realtime;
			probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
			probe.resolution = ProbeResolution;
			probe.cullingMask = CullingMask;
			probe.clearFlags = ReflectionProbeClearFlags.SolidColor;
			probe.backgroundColor = new Color(0f, 0f, 0f, 1f);
			probe.boxProjection = false;
			probe.importance = 1;
			probe.size = new Vector3(5000f, 5000f, 5000f);
		}

		private void LateUpdate()
		{
			Camera camera = ResolveWorldCamera();
			if (camera == null)
			{
				return;
			}
			transform.position = camera.transform.position;
			if (Time.unscaledTime >= nextRenderTime)
			{
				nextRenderTime = Time.unscaledTime + SecondsBetweenRenders;
				probe.RenderProbe();
			}
		}

		// Camera.main is unreliable here: BOTH MenuCamera and GameCamera are
		// tagged MainCamera, so it can return the (static) menu camera.
		// Prefer the game's canonical camera; fall back to the lowest-depth
		// enabled camera (the world base camera, per UrpCameraStacker).
		private static Camera ResolveWorldCamera()
		{
			if (GameController.Instance != null && GameController.Instance.MainCamera != null && GameController.Instance.MainCamera.isActiveAndEnabled)
			{
				return GameController.Instance.MainCamera;
			}
			Camera[] array = Object.FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			Camera camera = null;
			foreach (Camera camera2 in array)
			{
				if (camera == null || camera2.depth < camera.depth)
				{
					camera = camera2;
				}
			}
			return camera;
		}
	}
}
