using System;
using Pixelfactor.IP.Common;
using Pixelfactor.Unity.Utils;

namespace Pixelfactor.IP.SpaceUnity
{
	[Serializable]
	public class SpaceConstructorNebulaColourSetting : IWeighted
	{
		public NebulaColour NebulaColours;

		public float Weighting;

		public float Weight => Weighting;
	}
}
