using System;
using UnityEngine;

namespace OpenFrontier.IP
{
	public static class Versioning
	{
		private static Version version;

		// Open Frontier: the version comes from the project's bundle version
		// (Application.version / PlayerSettings), so the displayed and saved
		// version always matches the build - one number to bump.
		public static Version Version
		{
			get
			{
				if (version == null)
				{
					version = Parse(Application.version);
				}
				return version;
			}
		}

		private static Version Parse(string text)
		{
			try
			{
				return new Version(text);
			}
			catch (Exception)
			{
				return new Version(0, 0, 1);
			}
		}

		public static bool IsAlphaVersion = false;
	}
}
