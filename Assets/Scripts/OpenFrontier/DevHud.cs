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

		private float fpsAccum;
		private int fpsFrames;
		private float currentFps;

		private void Update()
		{
			fpsAccum += Time.unscaledDeltaTime;
			fpsFrames++;
			if (fpsAccum >= 0.5f)
			{
				currentFps = fpsFrames / fpsAccum;
				fpsAccum = 0f;
				fpsFrames = 0;
			}
		}

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
			// Scale up for high-DPI phone panels so the text is readable.
			float scale = Mathf.Max(2f, Screen.dpi > 0f ? Screen.dpi / 160f : 2f);
			Matrix4x4 old = GUI.matrix;
			GUI.matrix = Matrix4x4.Scale(new Vector3(scale, scale, 1f));
			Rect safe = Screen.safeArea;
			Vector2Int jni = AndroidDisplayMetrics.GetWindowSize();
			string hz = "";
#if UNITY_ANDROID && !UNITY_EDITOR
			hz = $"\nModes {AndroidDisplayMetrics.GetSupportedModesSummary()}  now {AndroidDisplayMetrics.GetCurrentRefreshRate():0}Hz";
#endif
			GUI.Box(new Rect(8f, 8f, 340f, 154f),
				$"FPS {currentFps:0}  (cap {Application.targetFrameRate}, vSync {QualitySettings.vSyncCount})\n" +
				$"Screen {Screen.width}x{Screen.height}  {Screen.orientation}\n" +
				$"Display.main {Display.main.systemWidth}x{Display.main.systemHeight}\n" +
				$"Android window {jni.x}x{jni.y}  dpi {Screen.dpi:0}\n" +
				$"SafeArea {safe.x:0},{safe.y:0} {safe.width:0}x{safe.height:0}\n" +
				$"resyncs {DisplayResyncCount}" + hz);
			GUI.matrix = old;
		}
	}
}
