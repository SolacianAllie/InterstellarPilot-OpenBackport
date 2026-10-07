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
	/// (cheap: small resolution, sparse layers), or on demand via
	/// RequestRender (rate-limited to 0.1s between renders).
	/// Spawned by ActiveSectorData under the EngineASX root.
	/// </summary>
	public class SpaceReflectionProbe : MonoBehaviour
	{
		public static SpaceReflectionProbe Instance { get; private set; }

		private const float SecondsBetweenRenders = 1f;

		private const float MinSecondsBetweenRequestedRenders = 0.1f;

		// DeepSpace (30) | BackgroundPlanet (26) | Wormhole (22) |
		// BackgroundObjects (16 - the sun's layer)
		private const int CullingMask = 1073741824 | 67108864 | 4194304 | 65536;

		private ReflectionProbe probe;

		private float nextRenderTime;

		private float lastRenderTime = -1f;

		private bool renderRequested;

		private void Awake()
		{
			Instance = this;
			probe = gameObject.AddComponent<ReflectionProbe>();
			probe.mode = ReflectionProbeMode.Realtime;
			probe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
			// Mobile gets a cheaper probe; reflections are blurry by nature.
			probe.resolution = (Application.isMobilePlatform ? 64 : 128);
			probe.cullingMask = CullingMask;
			probe.clearFlags = ReflectionProbeClearFlags.SolidColor;
			probe.backgroundColor = new Color(0f, 0f, 0f, 1f);
			probe.boxProjection = false;
			probe.importance = 1;
			probe.size = new Vector3(5000f, 5000f, 5000f);
		}

		private void OnDestroy()
		{
			if (Instance == this)
			{
				Instance = null;
			}
		}

		// Ask for a fresh render (e.g. godmode appearance edits). Renders
		// happen at most once per 0.1 seconds regardless of call rate.
		public void RequestRender()
		{
			renderRequested = true;
		}

		private void LateUpdate()
		{
			Camera camera = WorldCamera.Resolve();
			if (camera == null)
			{
				return;
			}
			transform.position = camera.transform.position;
			bool flag = renderRequested && Time.unscaledTime >= lastRenderTime + MinSecondsBetweenRequestedRenders;
			if (flag || Time.unscaledTime >= nextRenderTime)
			{
				renderRequested = false;
				lastRenderTime = Time.unscaledTime;
				nextRenderTime = lastRenderTime + SecondsBetweenRenders;
				probe.RenderProbe();
			}
		}
	}
}
