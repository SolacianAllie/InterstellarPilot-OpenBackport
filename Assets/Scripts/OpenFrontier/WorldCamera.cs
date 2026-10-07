using UnityEngine;

namespace OpenFrontier
{
	/// <summary>
	/// Open Frontier: reliable world-camera lookup. Camera.main is NOT
	/// reliable in this project - both MenuCamera and GameCamera are tagged
	/// MainCamera, so it can return the (static) menu camera. The URP base
	/// camera is always the lowest-depth enabled camera (UrpCameraStacker
	/// sorts the stack by depth), and only one context camera is enabled at
	/// a time, so depth order resolves it deterministically.
	/// </summary>
	public static class WorldCamera
	{
		public static Camera Resolve()
		{
			Camera[] array = Object.FindObjectsByType<Camera>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			Camera camera = null;
			foreach (Camera camera2 in array)
			{
				if (camera == null || camera2.depth < camera.depth)
				{
					camera = camera2;
				}
			}
			if (camera == null)
			{
				camera = Camera.main;
			}
			return camera;
		}
	}
}
