using UnityEngine;
using UnityEngine.Rendering;

namespace OpenFrontier.IP.Engine
{
	/// <summary>
	/// Open Frontier: realtime reflection probe positioned at the SECTOR
	/// ANCHOR - the same viewpoint the pinned SpaceCamera renders the sky
	/// from. The starfield is a 20000-wide cube at the world origin and
	/// the nebulae are world-pinned too; a camera-following probe leaves
	/// the star cube in far sectors and captures a lopsided sky. From the
	/// anchor the sky captures correctly, and since cubemaps are sampled
	/// by direction only, one anchor capture serves the whole sector.
	/// The mask spans sky AND local content (ships, stations, wormholes,
	/// planets, asteroids, gas clouds, the probe-only anchor sun on
	/// SkyProbe/3) so hulls reflect everything; local objects sit far
	/// enough from the anchor that the parallax error is sub-degree.
	/// NOT in the mask: BackgroundObjects (16) - that is the CAMERA-origin
	/// sun, which sits off-axis from the anchor.
	/// Clears to opaque black; re-renders once per second or on demand via
	/// RequestRender (rate-limited to 0.1s). Spawned by ActiveSectorData.
	/// </summary>
	public class SpaceReflectionProbe : MonoBehaviour
	{
		public static SpaceReflectionProbe Instance { get; private set; }

		private const float SecondsBetweenRenders = 1f;

		private const float MinSecondsBetweenRequestedRenders = 0.1f;

		// DeepSpace (30) | UnitGasCloud (29) | AsteroidClusterObject (27) |
		// BackgroundPlanet (26) | Planet (23) | Wormhole (22) |
		// Asteroid (20) | Station (19) | Ship (18) | SkyProbe (3 - anchor sun)
		private const int CullingMask = 1073741824 | 536870912 | 134217728 | 67108864 | 8388608 | 4194304 | 1048576 | 524288 | 262144 | 8;

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
			// The default far plane (1000) clipped nearly everything we
			// want reflected; even 9000 is not enough. The starfield is a
			// 20000-wide cube mesh pinned at the WORLD ORIGIN (Imphenzia
			// StaticStars.LateUpdate), while sectors sit up to 16000+
			// units out - from there the star cube is 6k-36k units from
			// the probe. The SpaceCamera survives this with far plane
			// 100000; the probe needs the same reach. Depth precision is
			// irrelevant for an emissive backdrop.
			probe.farClipPlane = 100000f;
			probe.cullingMask = CullingMask;
			probe.clearFlags = ReflectionProbeClearFlags.SolidColor;
			probe.backgroundColor = new Color(0f, 0f, 0f, 1f);
			probe.boxProjection = false;
			probe.importance = 1;
			// Cover the whole sector so every renderer samples this probe.
			probe.size = new Vector3(30000f, 30000f, 30000f);
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
			// Anchor to the sector (the SpaceCamera's viewpoint); fall back
			// to the world camera when no sector is loaded.
			if (EngineASX.Instance != null && EngineASX.Instance.ActiveSector != null)
			{
				transform.position = EngineASX.Instance.ActiveSector.transform.position;
			}
			else
			{
				Camera camera = WorldCamera.Resolve();
				if (camera == null)
				{
					return;
				}
				transform.position = camera.transform.position;
			}
			bool flag = renderRequested && Time.unscaledTime >= lastRenderTime + MinSecondsBetweenRequestedRenders;
			if (flag || Time.unscaledTime >= nextRenderTime)
			{
				renderRequested = false;
				lastRenderTime = Time.unscaledTime;
				nextRenderTime = lastRenderTime + SecondsBetweenRenders;
				int num = probe.RenderProbe();
				if (num == -1)
				{
					Debug.LogWarning("[SpaceReflectionProbe] RenderProbe scheduling failed", this);
				}
			}
		}
	}
}
