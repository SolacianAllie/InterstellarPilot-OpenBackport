using UnityEngine;
using UnityEngine.Rendering;

namespace OpenFrontier.IP.Engine
{
	/// <summary>
	/// Open Frontier: TWO blended realtime reflection probes.
	///
	/// SKY probe (importance 1, sector anchor, far 100000, box 30000):
	/// captures only the sky - starfield + nebulae (DeepSpace/30), gas
	/// clouds (29) and the probe-only anchor sun (SkyProbe/3). The
	/// starfield is a 20000-wide cube pinned at the world origin and the
	/// nebulae are world-pinned too, so the sky only captures correctly
	/// from the sector anchor - the SpaceCamera's own viewpoint. Its huge
	/// box covers the whole sector so every renderer samples it.
	///
	/// NEAR probe (importance 2, follows the camera, far 9000, box 1000):
	/// captures local detail - the CAMERA-origin sun (BackgroundObjects/16,
	/// on-axis from here), ships, stations, asteroids, wormholes, planets.
	///
	/// URP reflection probe blending (enabled in the pipeline asset) mixes
	/// both per renderer by volume weight; deep inside both boxes the
	/// blend is 50/50, so both probes run intensity 2 to compensate -
	/// making the blend an exact additive composite: sky from one probe,
	/// local detail from the other, each at full strength.
	///
	/// Both clear to opaque black; re-render once per second or on demand
	/// via RequestRender (rate-limited to 0.1s). Spawned by
	/// ActiveSectorData under the EngineASX root.
	/// </summary>
	public class SpaceReflectionProbe : MonoBehaviour
	{
		public static SpaceReflectionProbe Instance { get; private set; }

		private const float SecondsBetweenRenders = 1f;

		private const float MinSecondsBetweenRequestedRenders = 0.1f;

		// SKY: DeepSpace (30 - starfield + nebulae) | UnitGasCloud (29) |
		// SkyProbe (3 - anchor sun)
		private const int SkyCullingMask = 1073741824 | 536870912 | 8;

		// NEAR: BackgroundObjects (16 - camera sun) | AsteroidClusterObject
		// (27) | BackgroundPlanet (26) | Planet (23) | Wormhole (22) |
		// Asteroid (20) | Station (19) | Ship (18)
		private const int NearCullingMask = 65536 | 134217728 | 67108864 | 8388608 | 4194304 | 1048576 | 524288 | 262144;

		// Both probes blend ~50/50 inside the overlap; intensity 2 makes
		// the blend an exact additive composite instead of half-dim.
		private const float ProbeIntensity = 2f;

		private ReflectionProbe skyProbe;

		private ReflectionProbe nearProbe;

		private float nextRenderTime;

		private float lastRenderTime = -1f;

		private bool renderRequested;

		private void Awake()
		{
			Instance = this;
			skyProbe = CreateProbe("SkyReflectionProbe", SkyCullingMask, 100000f, new Vector3(30000f, 30000f, 30000f), 1);
			nearProbe = CreateProbe("NearReflectionProbe", NearCullingMask, 9000f, new Vector3(1000f, 1000f, 1000f), 2);
		}

		private ReflectionProbe CreateProbe(string objectName, int cullingMask, float farClip, Vector3 size, int importance)
		{
			GameObject gameObject = new GameObject(objectName);
			gameObject.transform.SetParent(transform, worldPositionStays: false);
			ReflectionProbe reflectionProbe = gameObject.AddComponent<ReflectionProbe>();
			reflectionProbe.mode = ReflectionProbeMode.Realtime;
			reflectionProbe.refreshMode = ReflectionProbeRefreshMode.ViaScripting;
			// Mobile gets a cheaper probe; reflections are blurry by nature.
			reflectionProbe.resolution = (Application.isMobilePlatform ? 64 : 128);
			reflectionProbe.farClipPlane = farClip;
			reflectionProbe.cullingMask = cullingMask;
			reflectionProbe.clearFlags = ReflectionProbeClearFlags.SolidColor;
			reflectionProbe.backgroundColor = new Color(0f, 0f, 0f, 1f);
			reflectionProbe.boxProjection = false;
			reflectionProbe.intensity = ProbeIntensity;
			reflectionProbe.importance = importance;
			reflectionProbe.size = size;
			return reflectionProbe;
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
			// The sky probe sits at the sector anchor (the SpaceCamera's
			// viewpoint); fall back to the camera when no sector is loaded.
			if (EngineASX.Instance != null && EngineASX.Instance.ActiveSector != null)
			{
				skyProbe.transform.position = EngineASX.Instance.ActiveSector.transform.position;
			}
			else
			{
				Camera camera2 = WorldCamera.Resolve();
				if (camera2 != null)
				{
					skyProbe.transform.position = camera2.transform.position;
				}
			}
			Camera camera = WorldCamera.Resolve();
			if (camera == null)
			{
				return;
			}
			nearProbe.transform.position = camera.transform.position;
			bool flag = renderRequested && Time.unscaledTime >= lastRenderTime + MinSecondsBetweenRequestedRenders;
			if (flag || Time.unscaledTime >= nextRenderTime)
			{
				renderRequested = false;
				lastRenderTime = Time.unscaledTime;
				nextRenderTime = lastRenderTime + SecondsBetweenRenders;
				if (skyProbe.RenderProbe() == -1)
				{
					Debug.LogWarning("[SpaceReflectionProbe] Sky RenderProbe scheduling failed", this);
				}
				if (nearProbe.RenderProbe() == -1)
				{
					Debug.LogWarning("[SpaceReflectionProbe] Near RenderProbe scheduling failed", this);
				}
			}
		}
	}
}
