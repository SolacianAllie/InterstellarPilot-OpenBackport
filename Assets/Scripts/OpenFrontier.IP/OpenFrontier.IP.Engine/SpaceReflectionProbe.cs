using UnityEngine;
using UnityEngine.Rendering;

namespace OpenFrontier.IP.Engine
{
	/// <summary>
	/// Open Frontier: ONE realtime reflection probe at the SECTOR ANCHOR -
	/// the same viewpoint the pinned SpaceCamera renders the sky from.
	/// (URP no longer blends two probes the way BiRP did, so the sky/near
	/// split collapsed to a single anchor probe.)
	///
	/// The starfield is a 20000-wide cube pinned at the world origin and
	/// the nebulae are world-pinned too, so the sky only captures
	/// correctly from the anchor; since cubemaps are sampled by direction
	/// only, one anchor capture serves the whole sector. The mask spans
	/// the sky (DeepSpace/30: starfield + nebulae), gas clouds (29),
	/// planets (23) and background planets (26), plus the probe-only
	/// anchor sun (SkyProbe/3) - world objects sit far enough from the
	/// anchor that their parallax error is sub-degree.
	///
	/// Clears to opaque black; re-renders once per second or on demand via
	/// RequestRender (rate-limited to 0.1s). Spawned by
	/// ActiveSectorData under the EngineASX root.
	/// </summary>
	public class SpaceReflectionProbe : MonoBehaviour
	{
		public static SpaceReflectionProbe Instance { get; private set; }

		private const float SecondsBetweenRenders = 1f;

		private const float MinSecondsBetweenRequestedRenders = 0.1f;

		// DeepSpace (30 - starfield + nebulae) | UnitGasCloud (29) |
		// BackgroundPlanet (26) | Planet (23) | SkyProbe (3 - anchor sun)
		private const int SkyCullingMask = 1073741824 | 536870912 | 67108864 | 8388608 | 8;

		private ReflectionProbe skyProbe;

		private float nextRenderTime;

		private float lastRenderTime = -1f;

		private bool renderRequested;

		private void Awake()
		{
			Instance = this;
			skyProbe = CreateProbe("SkyReflectionProbe", SkyCullingMask, 100000f, new Vector3(30000f, 30000f, 30000f), 1);
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
			// The probe sits at the sector anchor (the SpaceCamera's
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
			bool flag = renderRequested && Time.unscaledTime >= lastRenderTime + MinSecondsBetweenRequestedRenders;
			if (flag || Time.unscaledTime >= nextRenderTime)
			{
				renderRequested = false;
				lastRenderTime = Time.unscaledTime;
				nextRenderTime = lastRenderTime + SecondsBetweenRenders;
				if (skyProbe.RenderProbe() == -1)
				{
					Debug.LogWarning("[SpaceReflectionProbe] RenderProbe scheduling failed", this);
				}
			}
		}
	}
}
