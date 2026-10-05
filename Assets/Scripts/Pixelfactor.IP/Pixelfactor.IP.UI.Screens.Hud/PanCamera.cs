using DigitalRubyShared;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.Settings;
using UnityEngine;

namespace Pixelfactor.IP.UI.Screens.Hud
{
	public class PanCamera : MonoBehaviour
	{
		public GameObject OrbitScaleGesturePlatformSpecificView;

		private float desiredCameraAngleY;

		public PanGestureRecognizer OrbitPanGesture { get; private set; }

		private void OnEnable()
		{
			OrbitPanGesture = new PanGestureRecognizer();
			OrbitPanGesture.StateUpdated += PanGestureUpdated;
			OrbitPanGesture.PlatformSpecificView = OrbitScaleGesturePlatformSpecificView;
			FingersScript.Instance.AddGesture(OrbitPanGesture);
		}

		private void PanGestureUpdated(GestureRecognizer r)
		{
			if (EngineASX.LoadedAndReady && EngineASX.Instance.Hud.IsCurrentScreen)
			{
				if (r.State == GestureRecognizerState.Began)
				{
					EngineASX.Instance.Hud.CameraMode = HudCameraMode.Free;
					desiredCameraAngleY = GameController.Instance.MainCamera.transform.eulerAngles.y;
				}
				if (r.State == GestureRecognizerState.Executing)
				{
					EngineASX.Instance.Hud.CameraMode = HudCameraMode.Free;
					float velocityX = r.VelocityX;
					float velocityY = r.VelocityY;
					velocityX *= RealTime.deltaTime;
					velocityY *= RealTime.deltaTime;
					HudCameraSettings hudCameraSettings = EngineASX.Instance.GameSettings.HudCameraSettings;
					float num = Mathf.Lerp(hudCameraSettings.OrbitPanGestureMinRotationSpeed, hudCameraSettings.OrbitPanGestureMaxRotationSpeed, GameController.Instance.CameraDragRotateSensitivity) * hudCameraSettings.OrbitPanGestureSpeedMultiplier;
					num *= GameController.Instance.GetCameraDragRotateSensitivityMultiplier();
					desiredCameraAngleY += DeviceInfo.PixelsToUnits(velocityX) * num;
					float num2 = Mathf.Lerp(hudCameraSettings.OrbitPanGestureMinElevationSpeed, hudCameraSettings.OrbitPanGestureMaxElevationSpeed, GameController.Instance.CameraDragRotateSensitivity) * hudCameraSettings.OrbitPanGestureSpeedMultiplier;
					EngineASX.Instance.HudCamera.CameraElevation -= DeviceInfo.PixelsToUnits(velocityY) * num2;
					EngineASX.Instance.HudCamera.SetCameraDesiredRotationY(desiredCameraAngleY);
				}
			}
		}

		private void OnDisable()
		{
			if (FingersScript.HasInstance)
			{
				FingersScript.Instance.RemoveGesture(OrbitPanGesture);
			}
		}
	}
}
