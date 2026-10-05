using System;
using System.Collections.Generic;

namespace Pixelfactor.IP.Avatars
{
	[Serializable]
	public class AvatarSettingsChannelColours
	{
		public AvatarAssetColourChannel ColourChannel;

		public List<AvatarSettingsChannelColour> WeightedColours;
	}
}
