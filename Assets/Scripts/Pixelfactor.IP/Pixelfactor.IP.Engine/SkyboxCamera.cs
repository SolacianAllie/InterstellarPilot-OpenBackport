using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public class SkyboxCamera : MonoBehaviour
	{
		public Skybox Skybox;

		public Renderer SkyOverlayRenderer;

		// Open Frontier: the gamma-authored sky tint reads far too bright when
		// alpha-blended in linear color space. Normalize each cloud tint to a
		// target brightness (hue preserved) so the sky matches the fog darkness.
		// 0.30 ≈ hex 4D4A48.
		public float SkyOverlayTargetBrightness = 0.3f;

		public void SetSkyOverlayColor(Color color)
		{
			float lum = color.r * 0.2126f + color.g * 0.7152f + color.b * 0.0722f;
			float scale = lum > 0.0001f ? SkyOverlayTargetBrightness / lum : 1f;
			scale = Mathf.Min(scale, 1f); // never brighten tints already darker than target
			SkyOverlayRenderer.material.color = new Color(color.r * scale, color.g * scale, color.b * scale, color.a);
		}

		public void SetTint(Color color)
		{
		}

		public void SetExposure(float blend)
		{
		}
	}
}
