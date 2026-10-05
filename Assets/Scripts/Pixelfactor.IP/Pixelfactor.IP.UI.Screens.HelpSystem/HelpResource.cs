using UnityEngine;

namespace Pixelfactor.IP.UI.Screens.HelpSystem
{
	public class HelpResource : MonoBehaviour
	{
		public string Title;

		[TextArea(8, 16)]
		public string Description;
	}
}
