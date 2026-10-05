using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI
{
	public class FlowLayoutBrokenLayoutFix : MonoBehaviour
	{
		private void Start()
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
		}
	}
}
