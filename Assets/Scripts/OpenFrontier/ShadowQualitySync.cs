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
			float num2 = ((qualityLevel == 0) ? 0f : 250f);
			if (universalRenderPipelineAsset.shadowDistance != num2)
			{
				universalRenderPipelineAsset.shadowDistance = num2;
			}
		}
	}
}
