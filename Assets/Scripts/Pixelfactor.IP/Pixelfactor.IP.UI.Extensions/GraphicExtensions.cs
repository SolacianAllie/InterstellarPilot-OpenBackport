using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Extensions
{
	public static class GraphicExtensions
	{
		public static void SetAlpha(this Graphic graphic, float value)
		{
			Color color = graphic.color;
			color.a = value;
			graphic.color = color;
		}

		public static float GetAlpha(this Graphic graphic)
		{
			return graphic.color.a;
		}
	}
}
