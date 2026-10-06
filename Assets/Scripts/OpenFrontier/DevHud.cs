using UnityEngine;

namespace OpenFrontier
{
	/// <summary>
	/// Development overlay showing live screen metrics, for foldable /
	/// display-resize debugging. Active in development builds and the
	/// editor only; release builds are unaffected.
	/// </summary>
	public class DevHud : MonoBehaviour
	{
		private static DevHud instance;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Create()
		{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
			if (instance == null)
			{
				GameObject go = new GameObject("DevHud");
				instance = go.AddComponent<DevHud>();
				Object.DontDestroyOnLoad(go);
			}
#endif
		}

		private void OnGUI()
		{
			Rect safe = Screen.safeArea;
			GUI.Box(new Rect(8f, 8f, 360f, 84f),
				$"Screen {Screen.width}x{Screen.height}  {Screen.orientation}\n" +
				$"SafeArea {safe.x:0},{safe.y:0} {safe.width:0}x{safe.height:0}\n" +
				$"targetFrameRate {Application.targetFrameRate}");
		}
	}
}
