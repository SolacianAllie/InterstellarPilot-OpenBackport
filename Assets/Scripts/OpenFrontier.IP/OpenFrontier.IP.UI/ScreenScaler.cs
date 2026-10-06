using System.Collections.Generic;
using OpenFrontier.IP.Engine;
using OpenFrontier.IP.UI.Screens;
using UnityEngine;
using UnityEngine.UI;

namespace OpenFrontier.IP.UI
{
	public class ScreenScaler : MonoBehaviour
	{
		[SerializeField]
		private ScreenNavigator screenNavigator;

		public ScreenNavigator ScreenNavigator
		{
			get
			{
				return screenNavigator;
			}
			set
			{
				if (screenNavigator != value)
				{
					ScreenNavigator oldScreenNavigator = screenNavigator;
					screenNavigator = value;
					OnScreenNavigatorChanged(oldScreenNavigator, screenNavigator);
				}
			}
		}

		private void Awake()
		{
			if (ScreenNavigator != null)
			{
				OnScreenNavigatorChanged(null, screenNavigator);
			}
		}

		private void Start()
		{
			if (ScreenNavigator == null)
			{
				ScreenNavigator = ScreenNavigator.Instance;
			}
			lastScreenSize = new Vector2Int(Screen.width, Screen.height);
		}

		// Open Frontier: foldable/display-swap support. Unity does not
		// raise an event when the device display resizes (fold/unfold),
		// so poll the size and re-apply UI scaling once it settles.
		private Vector2Int lastScreenSize;
		private bool settlePollRunning;

		private void Update()
		{
			if (!settlePollRunning && (Screen.width != lastScreenSize.x || Screen.height != lastScreenSize.y))
			{
				StartCoroutine(RescaleWhenSettled());
			}
		}

		private System.Collections.IEnumerator RescaleWhenSettled()
		{
			settlePollRunning = true;
			Vector2Int last = new Vector2Int(Screen.width, Screen.height);
			int stableFrames = 0;
			while (stableFrames < 5)
			{
				yield return null;
				if (Screen.width != last.x || Screen.height != last.y)
				{
					last = new Vector2Int(Screen.width, Screen.height);
					stableFrames = 0;
				}
				else
				{
					stableFrames++;
				}
			}
			lastScreenSize = new Vector2Int(Screen.width, Screen.height);
			ScaleScreens();
			Canvas.ForceUpdateCanvases();
			settlePollRunning = false;
		}

		public void ScaleScreens()
		{
			if ((bool)ScreenNavigator)
			{
				ScaleScreens(GameController.Instance.InterfaceReferenceHeight);
			}
		}

		public void ScaleScreens(float height)
		{
			ScaleScreen(UIController.Instance.HomeCanvas.GetComponent<CanvasScaler>(), height);
			foreach (ScreenBase loadedScreen in ScreenNavigator.LoadedScreens)
			{
				if (loadedScreen.CanvasScalers != null && loadedScreen.CanvasScalers.Count > 0)
				{
					ScaleScreen(loadedScreen.CanvasScalers, height);
				}
				else
				{
					ScaleScreen(loadedScreen.gameObject, height);
				}
			}
		}

		public void ScaleScreen(GameObject gameObject)
		{
			float interfaceReferenceHeight = GameController.Instance.InterfaceReferenceHeight;
			ScaleScreen(gameObject, interfaceReferenceHeight);
		}

		public void ScaleScreen(GameObject gameObject, float height)
		{
			ScaleScreen(gameObject.GetComponentsInChildren<CanvasScaler>(includeInactive: true), height);
		}

		public void ScaleScreen(IEnumerable<CanvasScaler> canvasScalers, float height)
		{
			foreach (CanvasScaler canvasScaler in canvasScalers)
			{
				ScaleScreen(canvasScaler, height);
			}
		}

		public void ScaleScreen(CanvasScaler canvasScaler, float height)
		{
			Vector2 referenceResolution = canvasScaler.referenceResolution;
			referenceResolution.y = height;
			canvasScaler.referenceResolution = referenceResolution;
		}

		private void OnScreenNavigatorChanged(ScreenNavigator oldScreenNavigator, ScreenNavigator newScreenNavigator)
		{
			if (oldScreenNavigator != null)
			{
				oldScreenNavigator.ScreenRegistered -= ScreenNavigator_ScreenRegistered;
			}
			if (screenNavigator != null)
			{
				screenNavigator.ScreenRegistered += ScreenNavigator_ScreenRegistered;
				ScaleScreens();
			}
		}

		private void ScreenNavigator_ScreenRegistered(ScreenNavigator sender, ScreenBase newScreen)
		{
			ScaleScreen(newScreen.gameObject, GameController.Instance.InterfaceReferenceHeight);
		}
	}
}
