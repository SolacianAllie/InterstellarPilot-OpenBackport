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

		// Incremented by the UI layer whenever it re-syncs the display
		// (fold/unfold); shown here so the behavior can be verified.
		public static int DisplayResyncCount;

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
			GUI.Box(new Rect(8f, 8f, 400f, 106f),
				$"Screen {Screen.width}x{Screen.height}  {Screen.orientation}\n" +
				$"Display.main {Display.main.systemWidth}x{Display.main.systemHeight}  dpi {Screen.dpi:0}\n" +
				$"SafeArea {safe.x:0},{safe.y:0} {safe.width:0}x{safe.height:0}\n" +
				$"targetFrameRate {Application.targetFrameRate}  resyncs {DisplayResyncCount}");
		}
	}
}
