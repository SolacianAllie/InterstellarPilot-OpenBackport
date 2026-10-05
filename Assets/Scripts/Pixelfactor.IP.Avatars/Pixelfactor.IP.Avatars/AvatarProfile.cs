using System.Collections.Generic;
using UnityEngine;

namespace Pixelfactor.IP.Avatars
{
	public class AvatarProfile : MonoBehaviour
	{
		public int Id;

		public string Name;

		public List<AvatarProfileLayer> ProfileLayers = new List<AvatarProfileLayer>();

		public AvatarColourSettings ColourSettings;
	}
}
