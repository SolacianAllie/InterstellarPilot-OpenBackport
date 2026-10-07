using UnityEngine;
using UnityEngine.Rendering;

namespace OpenFrontier
{
	/// <summary>
	/// Open Frontier: realtime reflection probe that captures only the space
	/// backdrop layers (DeepSpace, BackgroundPlanet, Wormhole), so ships and
	/// nearby objects reflect the starfield, sun and distant bodies instead
	/// of each other or nothing. Follows the main camera and re-renders via
	/// scripting once per second (cheap: small resolution, sparse layers).
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
			probe.clearFlags = ReflectionProbeClearFlags.Skybox;
			probe.boxProjection = false;
			probe.importance = 1;
			probe.size = new Vector3(5000f, 5000f, 5000f);
		}

		private void LateUpdate()
		{
			Camera main = Camera.main;
			if (main == null)
			{
				return;
			}
			transform.position = main.transform.position;
			if (Time.time >= nextRenderTime)
			{
				nextRenderTime = Time.time + SecondsBetweenRenders;
				probe.RenderProbe();
			}
		}
	}
}
