using Pixelfactor.IP.UI.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI
{
	public class ResetScrollPositionOnEnable : MonoBehaviour
	{
		private void OnEnable()
		{
			GetComponent<ScrollRect>().ResetScrollPosition();
		}
	}
}
