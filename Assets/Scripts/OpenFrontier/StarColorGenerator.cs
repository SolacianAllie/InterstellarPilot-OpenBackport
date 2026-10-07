using UnityEngine;

namespace OpenFrontier
{
	/// <summary>
	/// Open Frontier: deterministic per-sector star colors, seeded by the
	/// sector's UniqueId so every sector keeps its own star color across
	/// sessions. Palette is weighted towards near-white stars with the odd
	/// noticeably orange/blue/red one.
	/// </summary>
	public static class StarColorGenerator
	{
		// Resolves the sector's star color, in priority order:
		// 1. a StarColorMarker under the sector's own object (child of a
		//    scenario's Sector<name> object = that sector's crafted star)
		// 2. a scene-wide StarColorMarker
		// 3. the sector's DirectionLightColor when it carries a hue -
		//    author-painted in hand-crafted universes, or seeded into the
		//    field at universe creation (SectorCreator)
		// 4. the deterministic seeded color (fallback for old saves whose
		//    sectors predate the creation-time seeding)
		public static Color ResolveCurrent(GameObject sectorObject, Color sectorLightColor, int sectorUniqueId)
		{
			if (sectorObject != null)
			{
				StarColorMarker componentInChildren = sectorObject.GetComponentInChildren<StarColorMarker>();
				if (componentInChildren != null)
				{
					return componentInChildren.StarColor;
				}
			}
			if (StarColorMarker.ActiveMarker != null)
			{
				return StarColorMarker.ActiveMarker.StarColor;
			}
			if (IsChromatic(sectorLightColor))
			{
				return NormalizeStarColor(sectorLightColor);
			}
			return ForSector(sectorUniqueId);
		}

		// True when the value carries an actual hue (an author picked a
		// color, not just a brightness). The sector default (0.8 grey) and
		// brightness-only tweaks are achromatic.
		public static bool IsChromatic(Color color)
		{
			float num = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
			return num - Mathf.Min(color.r, Mathf.Min(color.g, color.b)) > 0.02f;
		}

		// Floors the brightness at 0.5 so the sun always reads, without
		// brightening anything already above it; HDR values pass through.
		public static Color NormalizeStarColor(Color color)
		{
			float num = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
			if (num >= 0.5f)
			{
				return color;
			}
			if (num <= 0f)
			{
				return new Color(0.5f, 0.5f, 0.5f, 1f);
			}
			float num2 = 0.5f / num;
			return new Color(color.r * num2, color.g * num2, color.b * num2, 1f);
		}

		public static void RgbToHsv(Color color, out float h, out float s, out float v)
		{
			float num = Mathf.Max(color.r, Mathf.Max(color.g, color.b));
			float num2 = Mathf.Min(color.r, Mathf.Min(color.g, color.b));
			float num3 = num - num2;
			v = num;
			s = ((num > 0f) ? (num3 / num) : 0f);
			if (num3 <= 0f)
			{
				h = 0f;
				return;
			}
			if (num == color.r)
			{
				h = (color.g - color.b) / num3;
			}
			else if (num == color.g)
			{
				h = 2f + (color.b - color.r) / num3;
			}
			else
			{
				h = 4f + (color.r - color.g) / num3;
			}
			h /= 6f;
			if (h < 0f)
			{
				h += 1f;
			}
		}

		public static Color ForSector(int sectorUniqueId)
		{
			System.Random random = new System.Random(sectorUniqueId * 7919 + 101);
			float num = (float)random.NextDouble();
			float h;
			float s;
			if (num < 0.55f)
			{
				// warm white / pale yellow (most common)
				h = Range(random, 0.08f, 0.14f);
				s = Range(random, 0.05f, 0.3f);
			}
			else if (num < 0.75f)
			{
				// blue-white
				h = Range(random, 0.55f, 0.62f);
				s = Range(random, 0.08f, 0.3f);
			}
			else if (num < 0.9f)
			{
				// orange
				h = Range(random, 0.03f, 0.08f);
				s = Range(random, 0.25f, 0.5f);
			}
			else
			{
				// red dwarf / vivid
				h = Range(random, 0f, 0.04f);
				s = Range(random, 0.4f, 0.65f);
			}
			// Brightness randomised too (floor 0.5 so the sun always reads).
			return HsvToRgb(h, s, Range(random, 0.5f, 1f));
		}

		private static float Range(System.Random random, float min, float max)
		{
			return min + (float)random.NextDouble() * (max - min);
		}

		public static Color HsvToRgb(float h, float s, float v)
		{
			if (s <= 0f)
			{
				return new Color(v, v, v, 1f);
			}
			float num = h * 6f;
			int num2 = (int)num % 6;
			float num3 = num - num2;
			float num4 = v * (1f - s);
			float num5 = v * (1f - s * num3);
			float num6 = v * (1f - s * (1f - num3));
			switch (num2)
			{
			case 0:
				return new Color(v, num6, num4, 1f);
			case 1:
				return new Color(num5, v, num4, 1f);
			case 2:
				return new Color(num4, v, num6, 1f);
			case 3:
				return new Color(num4, num5, v, 1f);
			case 4:
				return new Color(num6, num4, v, 1f);
			default:
				return new Color(v, num4, num5, 1f);
			}
		}
	}
}
