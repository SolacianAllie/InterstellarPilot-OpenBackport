using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OpenFrontier.IP.Engine
{
	/// <summary>
	/// Open Frontier: ties the URP shadow map resolution to the graphics
	/// quality slider - 4K at the highest level, 2K in the middle, 1K at
	/// the bottom. Lives on the persistent GameController and applies
	/// whenever the quality level changes.
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
		}
	}
}
