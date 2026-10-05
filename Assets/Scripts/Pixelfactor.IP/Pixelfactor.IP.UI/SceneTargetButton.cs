using UnityEngine;
using UnityEngine.SceneManagement;

namespace Pixelfactor.IP.UI
{
	public class SceneTargetButton : MonoBehaviour
	{
		public string TargetScene;

		private void OnClick()
		{
			if (!string.IsNullOrEmpty(TargetScene))
			{
				SceneManager.LoadScene(TargetScene, LoadSceneMode.Single);
			}
		}
	}
}
