using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public class SkyboxCamera : MonoBehaviour
	{
		public Skybox Skybox;

		public Renderer SkyOverlayRenderer;

		// Open Frontier: the gamma-authored sky tint reads far too bright when
		// alpha-blended in linear color space; scale its intensity down so the
		// sky darkens with the fog instead of glowing.
		public float SkyOverlayColorIntensity = 0.35f;

		public void SetSkyOverlayColor(Color color)
		{
			float i = SkyOverlayColorIntensity;
			SkyOverlayRenderer.material.color = new Color(color.r * i, color.g * i, color.b * i, color.a);
		}

		public void SetTint(Color color)
		{
		}

		public void SetExposure(float blend)
		{
		}
	}
}
