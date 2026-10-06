using UnityEngine;

namespace OpenFrontier
{
	/// <summary>
	/// Queries the real window size from Android's DisplayMetrics, bypassing
	/// UnityEngine.Screen, which can go stale on foldables when the display
	/// is swapped. On non-Android platforms it reports Screen.width/height.
	/// </summary>
	public static class AndroidDisplayMetrics
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		private static AndroidJavaClass unityPlayerClass;
#endif

		public static Vector2Int GetWindowSize()
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			try
			{
				if (unityPlayerClass == null)
				{
					unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				}
				using (AndroidJavaObject activity = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"))
				using (AndroidJavaObject resources = activity.Call<AndroidJavaObject>("getResources"))
				using (AndroidJavaObject metrics = resources.Call<AndroidJavaObject>("getDisplayMetrics"))
				{
					return new Vector2Int(metrics.Get<int>("widthPixels"), metrics.Get<int>("heightPixels"));
				}
			}
			catch (System.Exception)
			{
				return new Vector2Int(Screen.width, Screen.height);
			}
#else
			return new Vector2Int(Screen.width, Screen.height);
#endif
		}
	}
}
