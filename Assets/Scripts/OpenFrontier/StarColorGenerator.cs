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
			return HsvToRgb(h, s, 1f);
		}

		private static float Range(System.Random random, float min, float max)
		{
			return min + (float)random.NextDouble() * (max - min);
		}

		private static Color HsvToRgb(float h, float s, float v)
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
