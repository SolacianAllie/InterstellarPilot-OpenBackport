using UnityEngine;

namespace OpenFrontier.IP.Engine
{
	/// <summary>
	/// Open Frontier: continuously ensures the currently authoritative
	/// directional light has a SunBillboard. Lives on the persistent
	/// GameController, so it covers every flow - sandbox, manual missions
	/// with their own scene lights, scene swaps, lights toggled at runtime.
	/// (ActiveSectorData used to do this once on init/sector-change, which
	/// missed missions whose lighting is set up differently.)
	/// </summary>
	public class SunManager : MonoBehaviour
	{
		public Material CoreMaterial;

		public Material GlowMaterial;

		private const float SecondsBetweenChecks = 0.5f;

		private float nextCheckTime;

		private void Update()
		{
			if (Time.time >= nextCheckTime)
			{
				nextCheckTime = Time.time + SecondsBetweenChecks;
				EnsureSun();
			}
		}

		private void EnsureSun()
		{
			Light light = ((EngineASX.Instance != null) ? EngineASX.Instance.DirectionalLight : null);
			if (!IsUsable(light))
			{
				light = FirstActiveSceneDirectionalLight();
			}
			if (!IsUsable(light))
			{
				return;
			}
			SunBillboard sunBillboard = light.GetComponent<SunBillboard>();
			if (sunBillboard == null)
			{
				sunBillboard = light.gameObject.AddComponent<SunBillboard>();
				sunBillboard.CoreMaterial = CoreMaterial;
				sunBillboard.GlowMaterial = GlowMaterial;
				sunBillboard.AutoTintFromSector = true;
			}
			if (sunBillboard.AutoTintFromSector)
			{
				int sectorUniqueId = ((EngineASX.Instance != null && EngineASX.Instance.ActiveSector != null) ? EngineASX.Instance.ActiveSector.UniqueId : 0);
				sunBillboard.StarTint = StarColorGenerator.ForSector(sectorUniqueId);
			}
		}

		private static bool IsUsable(Light light)
		{
			return light != null && light.isActiveAndEnabled && light.type == LightType.Directional;
		}

		private static Light FirstActiveSceneDirectionalLight()
		{
			Light[] array = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			foreach (Light light in array)
			{
				if (light.type == LightType.Directional)
				{
					return light;
				}
			}
			return null;
		}
	}
}
