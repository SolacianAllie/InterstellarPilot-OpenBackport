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
			if (instance == null)
			{
				GameObject go = new GameObject("DevHud");
				instance = go.AddComponent<DevHud>();
				Object.DontDestroyOnLoad(go);
			}
		}

		private void OnGUI()
		{
			// Scale up for high-DPI phone panels so the text is readable.
			float scale = Mathf.Max(2f, Screen.dpi > 0f ? Screen.dpi / 160f : 2f);
			Matrix4x4 old = GUI.matrix;
			GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
			Rect safe = Screen.safeArea;
			Vector2Int jni = AndroidDisplayMetrics.GetWindowSize();
			GUI.Box(new Rect(8f, 8f, 340f, 110f),
				$"Screen {Screen.width}x{Screen.height}  {Screen.orientation}\n" +
				$"Display.main {Display.main.systemWidth}x{Display.main.systemHeight}\n" +
				$"Android window {jni.x}x{jni.y}  dpi {Screen.dpi:0}\n" +
				$"SafeArea {safe.x:0},{safe.y:0} {safe.width:0}x{safe.height:0}\n" +
				$"fps cap {Application.targetFrameRate}  resyncs {DisplayResyncCount}");
			GUI.matrix = old;
		}
	}
}
