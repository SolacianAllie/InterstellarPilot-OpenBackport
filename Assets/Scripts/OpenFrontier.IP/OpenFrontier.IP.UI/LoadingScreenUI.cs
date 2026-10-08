using OpenFrontier.IP.Engine;
using UnityEngine;

namespace OpenFrontier.IP.UI
{
	public class LoadingScreenUI : MonoBehaviour
	{
		private bool isDestroying;

		public LoadingWidgetUI LoadingWidget;

		private static LoadingScreenUI instance;

		public static LoadingScreenUI Instance => instance;

		private void Awake()
		{
			instance = this;
		}

		private void OnDestroy()
		{
			if (instance == this)
			{
				instance = null;
			}
		}

		private void Start()
		{
			Time.timeScale = 0f;
			if (GameController.Instance != null)
			{
				GameController.Instance.ScreenScaler.ScaleScreen(gameObject);
			}
		}

		private void Update()
		{
			// Open Frontier: hold the screen while the main menu world's
			// warm-up burst runs, so the time skip stays behind the
			// loading screen until it is done.
			if (!isDestroying && (GameController.Instance.ScenarioLoader == null || !GameController.Instance.ScenarioLoader.IsLoading) && EngineASX.LoadedAndReady && !MainMenuWorldController.WarmUpInProgress)
			{
				isDestroying = true;
				Object.Destroy(gameObject);
			}
			// Keep the full loading visuals (text + backdrop) up for the
			// warm-up too - without this the widget vanishes and the
			// screen reads as plain black until the burst ends.
			LoadingWidget.gameObject.SetActive(EngineASX.IsLoading || MainMenuWorldController.WarmUpInProgress);
		}
	}
}
