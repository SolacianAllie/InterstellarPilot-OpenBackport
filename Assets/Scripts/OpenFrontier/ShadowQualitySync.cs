using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OpenFrontier
{
	/// <summary>
	/// Open Frontier: ties the URP shadow settings to the graphics quality
	/// slider - no shadows at the lowest level, then 1K / 2K / 4K maps as
	/// quality rises. Lives on the persistent GameController and applies
	/// whenever the quality level changes. Shadows are disabled by zeroing
	/// the shadow distance (the supported-flags are not runtime-settable).
	/// </summary>
	public class ShadowQualitySync : MonoBehaviour
	{
		private int appliedLevel = -1;

		private void Awake()
		{
			// Spawn the post-processing ladder sibling here: it has no
			// prefab wiring of its own (avoids a new-script GUID dance).
			if (GetComponent<PostProcessingQualitySync>() == null)
			{
				gameObject.AddComponent<PostProcessingQualitySync>();
			}
		}

		private void Update()
		{
			int qualityLevel = QualitySettings.GetQualityLevel();
			if (qualityLevel != appliedLevel)
			{
				appliedLevel = qualityLevel;
				Apply(qualityLevel);
			}
		}

		private static void Apply(int qualityLevel)
		{
			UniversalRenderPipelineAsset universalRenderPipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
			if (universalRenderPipelineAsset == null)
			{
				return;
			}
			int num;
			switch (qualityLevel)
			{
			case 0:
			case 1:
				num = 1024;
				break;
			case 2:
			case 3:
				num = 2048;
				break;
			default:
				num = 4096;
				break;
			}
			if (universalRenderPipelineAsset.mainLightShadowmapResolution != num)
			{
				universalRenderPipelineAsset.mainLightShadowmapResolution = num;
			}
			// Tiers 2 (Normal) and 3 (High) stretch shadows to fill the
			// sector (gate distance ~3000u). The split is re-balanced so
			// the near field stays tight (30/150/600/3000) instead of
			// scaling the stock proportions (60/300/900 - too soft up
			// close). Lowest = off, Low/Highest = stock 250u.
			float num2;
			Vector3 vector;
			switch (qualityLevel)
			{
			case 0:
				num2 = 0f;
				vector = new Vector3(0.02f, 0.1f, 0.3f);
				break;
			case 2:
			case 3:
				num2 = 3000f;
				vector = new Vector3(0.01f, 0.05f, 0.2f);
				break;
			default:
				num2 = 250f;
				vector = new Vector3(0.02f, 0.1f, 0.3f);
				break;
			}
			if (universalRenderPipelineAsset.shadowDistance != num2)
			{
				universalRenderPipelineAsset.shadowDistance = num2;
			}
			if (universalRenderPipelineAsset.cascade4Split != vector)
			{
				universalRenderPipelineAsset.cascade4Split = vector;
			}
		}
	}
}
