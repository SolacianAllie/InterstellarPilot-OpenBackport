using System.Collections.Generic;
using System.Linq;
using OpenFrontier.IP.Engine;
using OpenFrontier.IP.UI.Screens;
using OpenFrontier.IP.UI.Screens.MessageBox;
using OpenFrontier.IP.UI.Screens.Options;
using OpenFrontier.IP.UI.Screens.RenameUnit;
using OpenFrontier.Unity.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Input = OpenFrontier.LegacyInput;

namespace OpenFrontier.IP.UI
{
	public class OptionsScreen : ScreenBase
	{
		public TMP_Dropdown ScreenResolutionDropdown;

		public TMP_Dropdown TargetFrameRateDropdown;

		public Toggle FullScreenToggle;

		public Toggle General_AllowFireAtNeutralToggle;

		public Toggle General_AllowFireAtAlliedToggle;

		public Toggle ShowPointDefenceTurretsToggle;

		public Toggle AutoHideShieldsToggle;

		public Toggle ScreenSpaceShieldsToggle;

		public Toggle WorldSpaceShieldsToggle;

		public DifficultySlider CombatDifficultySlider;

		public Button AdjustSafeAreaButton;

		public GameObject TimeAutoSaveOptionRoot;

		public Text GraphicsQualityText;

		public Slider GraphicsQualitySlider;

		public Toggle AutoCloseInGameMenuToggle;

		public Toggle ButtonSoundsToggle;

		public Toggle MissileLockSoundToggle;

		public Button CalibrateTiltButton;

		public Slider InterfaceSizeSlider;

		public Slider MasterVolumeSlider;

		public Slider MusicVolumeSlider;

		public Toggle PreferCameraLockToggle;

		public Toggle AutoTargetHostilesToggle;

		public Toggle PreferRespawnOnDeathToggle;

		private float reizeUiTime;

		private bool resizeUi;

		public Button SaveButton;

		public Toggle SpaceFogEnabledToggle;

		public Toggle RenderCloakedShipOutlinesEnabledToggle;

		public Toggle WormholeAnimationToggle;

		public Toggle LightingFXToggle;

		public Toggle TiltControlEnabledToggle;

		public Toggle ForceTouchControlEnabledToggle;

		public Toggle AutoSaveOnWormholeExitToggle;

		public Toggle AutoSaveOnDockingToggle;

		public Toggle AutoSaveAfterDurationToggle;

		public Slider TiltSensitivitySlider;

		public Slider TouchTurnSensitivitySlider;

		public Slider CameraDragRotationSensitivitySlider;

		public Button DefaultPilotNameButton;

		public Text DefaultPilotNameLabel;

		public Button DefaultPilotTitleButton;

		public Text DefaultPilotTitleLabel;

		public Button DefaultFactionNameButton;

		public Text DefaultFactionNameLabel;

		public Button DefaultFactionShortNameButton;

		public Text DefaultFactionShortNameLabel;

		public Toggle CameraShakeOnShieldHitToggle;

		public Toggle CameraShakeOnHullHitToggle;

		private Resolution[] supportedResolutions;

		// Open Frontier: index of the synthetic 30Hz battery-saver entry
		// in the resolution dropdown (Android only, -1 when absent).
		private int batterySaverIndex = -1;

		protected override void awake()
		{
			base.awake();
			if (SaveButton != null)
			{
				SaveButton.onClick.AddListener(Save);
			}
			MissileLockSoundToggle.onValueChanged.AddListener(MissileLockSoundToggleValueChanged);
			ButtonSoundsToggle.onValueChanged.AddListener(ButtonSoundsToggle_ValueChanged);
			AutoCloseInGameMenuToggle.onValueChanged.AddListener(AutoCloseInGameMenuToggleValueChanged);
			AutoHideShieldsToggle.onValueChanged.AddListener(AutoHideShieldsToggleValueChanged);
			ShowPointDefenceTurretsToggle.onValueChanged.AddListener(ShowPointDefenceTurretsToggleValueChanged);
			General_AllowFireAtNeutralToggle.onValueChanged.AddListener(General_AllowFireAtNeutralToggleValueChanged);
			General_AllowFireAtAlliedToggle.onValueChanged.AddListener(General_General_AllowFireAtAlliedChanged);
			ScreenSpaceShieldsToggle.onValueChanged.AddListener(ScreenSpaceShieldsToggleValueChanged);
			WorldSpaceShieldsToggle.onValueChanged.AddListener(WorldSpaceShieldsToggleValueChanged);
			if (TouchTurnSensitivitySlider != null)
			{
				TouchTurnSensitivitySlider.onValueChanged.AddListener(TouchTurnSensitivitySlider_ValueChanged);
			}
			TiltSensitivitySlider.onValueChanged.AddListener(TiltSensitivitySlider_ValueChanged);
			TiltControlEnabledToggle.onValueChanged.AddListener(TiltControlToggle_ValueChanged);
			ForceTouchControlEnabledToggle.onValueChanged.AddListener(ForceTouchControlEnabledToggle_ValueChanged);
			AutoSaveOnDockingToggle.onValueChanged.AddListener(AutoSaveOnDockingToggleValueChanged);
			AutoSaveOnWormholeExitToggle.onValueChanged.AddListener(AutoSaveOnWormholeExitToggleValueChanged);
			AutoSaveAfterDurationToggle.onValueChanged.AddListener(AutoSaveAfterDurationToggleValueChanged);
			CameraDragRotationSensitivitySlider.onValueChanged.AddListener(CameraDragRotationSensitivitySliderValueChanged);
			AddAutoSaveAfterDurationMinutesHandlers();
			MasterVolumeSlider.onValueChanged.AddListener(MasterVolumeSlider_ValueChanged);
			MusicVolumeSlider.onValueChanged.AddListener(MusicVolumeSlider_ValueChanged);
			if (InterfaceSizeSlider != null)
			{
				InterfaceSizeSlider.onValueChanged.AddListener(InterfaceSizeSlider_ValueChanged);
			}
			PreferCameraLockToggle.onValueChanged.AddListener(PreferCameraToggle_ValueChanged);
			AutoTargetHostilesToggle.onValueChanged.AddListener(AutoTargetHostilesToggleValueChanged);
			PreferRespawnOnDeathToggle.onValueChanged.AddListener(PreferRespawnOnDeathToggleValueChanged);
			CalibrateTiltButton.onClick.AddListener(CalibrateTiltButton_Activated);
			SpaceFogEnabledToggle.onValueChanged.AddListener(SpaceFogEnabledToggle_ValueChanged);
			RenderCloakedShipOutlinesEnabledToggle.onValueChanged.AddListener(RenderCloakedShipOutlinesEnabledToggle_ValueChanged);
			WormholeAnimationToggle.onValueChanged.AddListener(WormholeAnimationToggle_ValueChanged);
			LightingFXToggle.onValueChanged.AddListener(LightingFXToggleValueChanged);
			string[] names = QualitySettings.names;
			GraphicsQualitySlider.minValue = 0f;
			GraphicsQualitySlider.maxValue = Mathf.Max(0, names.Length - 1);
			GraphicsQualitySlider.wholeNumbers = true;
			GraphicsQualitySlider.onValueChanged.AddListener(GraphicsQualitySliderValueChanged);
			DefaultPilotNameButton.onClick.AddListener(DefaultPilotNameButtonClick);
			DefaultPilotTitleButton.onClick.AddListener(DefaultPilotTitleButtonClick);
			DefaultFactionNameButton.onClick.AddListener(DefaultFactionNameButtonClick);
			DefaultFactionShortNameButton.onClick.AddListener(DefaultFactionShortNameButtonClick);
			ForceTouchControlEnabledToggle.gameObject.SetActive(!UI.IsMobileDevice);
			AdjustSafeAreaButton.onClick.AddListener(AdjustSafeAreaButtonClick);
			FullScreenToggle.onValueChanged.AddListener(FullScreenToggleValueChanged);
			TargetFrameRateDropdown.gameObject.SetActive(value: false);
			TiltControlEnabledToggle.gameObject.SetActive(value: false);
			TargetFrameRateDropdown.onValueChanged.AddListener(TargetFrameRateDropdownValueChanged);
			CameraShakeOnHullHitToggle.onValueChanged.AddListener(CameraShakeOnHullHitToggleValueChanged);
			CameraShakeOnShieldHitToggle.onValueChanged.AddListener(CameraShakeOnShieldHitToggleValueChanged);
		}

		private void TargetFrameRateDropdownValueChanged(int value)
		{
			if (value >= 0 && value < GameController.Instance.PlayerOptionConstants.Video_TargetFrameRates.Length)
			{
				int fps = GameController.Instance.PlayerOptionConstants.Video_TargetFrameRates[value];
#if UNITY_ANDROID && !UNITY_EDITOR
				// Open Frontier: never cap above the panel's refresh rate.
				int refresh = Mathf.RoundToInt((float)Screen.currentResolution.refreshRateRatio.value);
				if (refresh > 0)
				{
					fps = Mathf.Min(fps, refresh);
				}
#endif
				Application.targetFrameRate = fps;
			}
		}

		private void ScreenResolutionDropdownValueChanged(int value)
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			if (batterySaverIndex >= 0 && value == batterySaverIndex)
			{
				// Synthetic battery-saver entry: keep the current display
				// mode (30Hz is not a real mode) and only cap the frame rate.
				GameController.Instance.ApplyMobileFrameRateCap(30);
				return;
			}
#endif
			if (value >= 0 && value < supportedResolutions.Length)
			{
				Screen.SetResolution(supportedResolutions[value].width, supportedResolutions[value].height, GameController.Instance.GetFullScreenMode(FullScreenToggle.isOn), supportedResolutions[value].refreshRate);
				GameController.Instance.ApplyMobileFrameRateCap(supportedResolutions[value].refreshRate);
			}
		}

		private void FullScreenToggleValueChanged(bool value)
		{
			GameController.Instance.ApplyResolution(value);
		}

		private void CameraShakeOnHullHitToggleValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.PlayerOptions.General_CameraShakeOnHullHit = value;
			}
		}

		private void CameraShakeOnShieldHitToggleValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.PlayerOptions.General_CameraShakeOnShieldHit = value;
			}
		}

		private void AdjustSafeAreaButtonClick()
		{
			UIController.Instance.ScreenNavigator.ShowAdjustSafeAreaScreen();
		}

		private void AddAutoSaveAfterDurationMinutesHandlers()
		{
			TimedAutoSaveOption[] componentsInChildren = TimeAutoSaveOptionRoot.GetComponentsInChildren<TimedAutoSaveOption>(includeInactive: true);
			foreach (TimedAutoSaveOption timedAutoSaveOption in componentsInChildren)
			{
				Toggle component = timedAutoSaveOption.GetComponent<Toggle>();
				if (component != null)
				{
					int minutes = timedAutoSaveOption.Minutes;
					component.onValueChanged.AddListener((bool value) =>
					{
						TimedAutoSaveOptionValueChanged(value, minutes);
					});
				}
			}
		}

		private void DefaultPilotNameButtonClick()
		{
			UIController.Instance.ScreenNavigator.ShowRenameScreen(GameController.Instance.DefaultPilotName, GameController.Instance.GameSettings.PilotNameMinChars, GameController.Instance.GameSettings.PilotNameMaxChars, OnDefaultPilotNameConfirmed);
		}

		private void DefaultPilotTitleButtonClick()
		{
			UIController.Instance.ScreenNavigator.ShowRenameScreen(GameController.Instance.DefaultPilotTitle, GameController.Instance.GameSettings.PilotTitleMinChars, GameController.Instance.GameSettings.PilotTitleMaxChars, OnDefaultPilotTitleConfirmed);
		}

		private void DefaultFactionNameButtonClick()
		{
			UIController.Instance.ScreenNavigator.ShowRenameScreen(GameController.Instance.DefaultFactionName, GameController.Instance.GameSettings.FactionMinNameChars, GameController.Instance.GameSettings.FactionMaxLongNameChars, OnRenameFactionNameConfirmed);
		}

		private void DefaultFactionShortNameButtonClick()
		{
			UIController.Instance.ScreenNavigator.ShowRenameScreen(GameController.Instance.DefaultFactionShortName, GameController.Instance.GameSettings.FactionMinShortNameChars, GameController.Instance.GameSettings.FactionMaxShortNameChars, OnRenameFactionShortNameConfirmed);
		}

		private void OnRenameFactionNameConfirmed(RenameUnitScreen handler, bool rename, string newName)
		{
			if (rename)
			{
				GameController.Instance.DefaultFactionName = newName;
				RefreshDefaultFactionNameLabel();
			}
		}

		private void OnRenameFactionShortNameConfirmed(RenameUnitScreen handler, bool rename, string newName)
		{
			if (rename)
			{
				GameController.Instance.DefaultFactionShortName = newName;
				RefreshDefaultFactionShortNameLabel();
			}
		}

		private void OnDefaultPilotNameConfirmed(RenameUnitScreen handler, bool rename, string newName)
		{
			if (rename)
			{
				GameController.Instance.DefaultPilotName = newName;
				RefreshDefaultPilotNameLabel();
			}
		}

		private void OnDefaultPilotTitleConfirmed(RenameUnitScreen handler, bool rename, string newName)
		{
			if (rename)
			{
				GameController.Instance.DefaultPilotTitle = newName;
				RefreshDefaultPilotTitleLabel();
			}
		}

		private void RefreshGraphicsQualityText()
		{
			int num = (int)GraphicsQualitySlider.value;
			string[] names = QualitySettings.names;
			if (num >= 0 && num < names.Length)
			{
				GraphicsQualityText.text = names[num];
			}
		}

		protected override void refresh()
		{
			base.refresh();
			if (GameController.Instance != null)
			{
				TargetFrameRateDropdown.onValueChanged.RemoveAllListeners();
				TargetFrameRateDropdown.ClearOptions();
				TargetFrameRateDropdown.AddOptions((from e in GameController.Instance.PlayerOptionConstants.Video_TargetFrameRates
					orderby e
					select e.ToString()).ToList());
				TargetFrameRateDropdown.value = GameController.Instance.PlayerOptionConstants.Video_TargetFrameRates.IndexOf(Application.targetFrameRate);
				TargetFrameRateDropdown.onValueChanged.AddListener(TargetFrameRateDropdownValueChanged);
				FullScreenToggle.isOn = Screen.fullScreen;
				RefreshScreenResolutionDropdown();
				MasterVolumeSlider.value = PlayerPrefsHelper.SafeGetFloat(GameController.Instance.MasterVolumeKey, 1f);
				MusicVolumeSlider.value = PlayerPrefsHelper.SafeGetFloat(GameController.Instance.MusicVolumeKey, GameController.Instance.DefaultMusicVolume);
				ButtonSoundsToggle.isOn = GameController.Instance.PlayButtonSounds;
				MissileLockSoundToggle.isOn = GameController.Instance.PlayerOptions.Audio_MissileLockSound;
				AutoCloseInGameMenuToggle.isOn = GameController.Instance.AutoCloseInGameMenu;
				SpaceFogEnabledToggle.isOn = GameController.Instance.SpaceFogEnabled;
				WormholeAnimationToggle.isOn = GameController.Instance.PlayerOptions.Video_WormholeAnimationEnabled;
				RenderCloakedShipOutlinesEnabledToggle.isOn = GameController.Instance.RenderCloakedShipOutlinesEnabled;
				LightingFXToggle.isOn = GameController.Instance.LightingFXEnabled;
				ForceTouchControlEnabledToggle.isOn = GameController.Instance.ForceTouchInputEnabled;
				TiltControlEnabledToggle.isOn = GameController.Instance.TiltControlEnabled;
				PreferCameraLockToggle.isOn = GameController.Instance.PreferCameraLock;
				AutoTargetHostilesToggle.isOn = GameController.Instance.AutoTargetHostiles;
				PreferRespawnOnDeathToggle.isOn = GameController.Instance.RespawnOnDeath;
				AutoHideShieldsToggle.isOn = GameController.Instance.PlayerOptions.UI_AutoHideShields;
				ScreenSpaceShieldsToggle.isOn = GameController.Instance.PlayerOptions.UI_ScreenSpaceShields;
				WorldSpaceShieldsToggle.isOn = !GameController.Instance.PlayerOptions.UI_ScreenSpaceShields;
				ShowPointDefenceTurretsToggle.isOn = GameController.Instance.PlayerOptions.UI_ShowPointDefenceTurrets;
				General_AllowFireAtAlliedToggle.isOn = GameController.Instance.PlayerOptions.General_AllowFireAtAllied;
				General_AllowFireAtNeutralToggle.isOn = GameController.Instance.PlayerOptions.General_AllowFireAtNeutral;
				CameraShakeOnHullHitToggle.isOn = GameController.Instance.PlayerOptions.General_CameraShakeOnHullHit;
				CameraShakeOnShieldHitToggle.isOn = GameController.Instance.PlayerOptions.General_CameraShakeOnShieldHit;
				GraphicsQualitySlider.value = QualitySettings.GetQualityLevel();
				AutoSaveOnWormholeExitToggle.isOn = GameController.Instance.AutoSaveSettings.AutoSaveOnWormholeExit;
				AutoSaveOnDockingToggle.isOn = GameController.Instance.AutoSaveSettings.AutoSaveOnDocking;
				AutoSaveAfterDurationToggle.isOn = GameController.Instance.AutoSaveSettings.AutoSaveAfterDuration;
				TimeAutoSaveOptionRoot.SetActive(GameController.Instance.AutoSaveSettings.AutoSaveAfterDuration);
				RefreshAutoSaveDurationToggle();
				if (TouchTurnSensitivitySlider != null)
				{
					TouchTurnSensitivitySlider.value = 1f - GameController.Instance.TouchTurnRadiusNorm;
				}
				TiltSensitivitySlider.value = GameController.Instance.TiltSensitivityNorm;
				if (InterfaceSizeSlider != null)
				{
					InterfaceSizeSlider.value = 1f - GameController.Instance.CanvasHeight01;
				}
				CameraDragRotationSensitivitySlider.value = GameController.Instance.CameraDragRotateSensitivity;
				RefreshGraphicsQualityText();
				RefreshDefaultPilotNameLabel();
				RefreshDefaultPilotTitleLabel();
				RefreshDefaultFactionNameLabel();
				RefreshDefaultFactionShortNameLabel();
				CombatDifficultySlider.Init();
				CombatDifficultySlider.SetDifficultyLevelIndex(GameController.Instance.CombatDifficultyLevelIndex);
			}
		}

		private void RefreshScreenResolutionDropdown()
		{
			ScreenResolutionDropdown.onValueChanged.RemoveAllListeners();
			ScreenResolutionDropdown.ClearOptions();
#if UNITY_ANDROID && !UNITY_EDITOR
			// Open Frontier: never cache on mobile - a fold swap changes
			// the panel's native resolution and available refresh rates.
			supportedResolutions = null;
#endif
			if (supportedResolutions == null)
			{
				supportedResolutions = Screen.resolutions.Where((Resolution e) => e.width >= GameController.Instance.GameSettings.VideoSettings.MinDisplayWidth && e.height >= GameController.Instance.GameSettings.VideoSettings.MinDisplayHeight).ToArray();
			}
			List<string> labels = supportedResolutions.Select((Resolution e) => $"{e.width:N0} x {e.height:N0} @ {e.refreshRate}Hz ({(float)e.width / (float)e.height:N2})").ToList();
			batterySaverIndex = -1;
#if UNITY_ANDROID && !UNITY_EDITOR
			// Open Frontier: inject a synthetic 30Hz battery-saver entry.
			// 30Hz is not a real display mode on most panels, so the change
			// handler only caps the frame rate instead of switching modes.
			if (supportedResolutions.Length > 0)
			{
				Resolution native = supportedResolutions[0];
				labels.Add($"{native.width:N0} x {native.height:N0} @ 30Hz ({(float)native.width / (float)native.height:N2}) (Battery Saver)");
				batterySaverIndex = labels.Count - 1;
			}
#endif
			ScreenResolutionDropdown.AddOptions(labels);
			Resolution item = supportedResolutions.FirstOrDefault((Resolution e) => e.width == Screen.width && e.height == Screen.height && e.refreshRate == Screen.currentResolution.refreshRate);
			ScreenResolutionDropdown.value = supportedResolutions.IndexOf(item);
#if UNITY_ANDROID && !UNITY_EDITOR
			if (batterySaverIndex >= 0 && Application.targetFrameRate > 0 && Application.targetFrameRate <= 30)
			{
				ScreenResolutionDropdown.value = batterySaverIndex;
			}
#endif
			ScreenResolutionDropdown.onValueChanged.AddListener(ScreenResolutionDropdownValueChanged);
		}

		private void RefreshAutoSaveDurationToggle()
		{
			Toggle[] componentsInChildren = TimeAutoSaveOptionRoot.GetComponentsInChildren<Toggle>();
			foreach (Toggle toggle in componentsInChildren)
			{
				TimedAutoSaveOption component = toggle.GetComponent<TimedAutoSaveOption>();
				if (component != null)
				{
					toggle.isOn = component.Minutes == GameController.Instance.AutoSaveSettings.AutoSaveAfterDurationMinutes;
				}
			}
		}

		private void RefreshDefaultFactionNameLabel()
		{
			DefaultFactionNameLabel.text = GameController.Instance.DefaultFactionName;
		}

		private void RefreshDefaultFactionShortNameLabel()
		{
			DefaultFactionShortNameLabel.text = GameController.Instance.DefaultFactionShortName;
		}

		private void RefreshDefaultPilotNameLabel()
		{
			DefaultPilotNameLabel.text = GameController.Instance.DefaultPilotName;
		}

		private void RefreshDefaultPilotTitleLabel()
		{
			DefaultPilotTitleLabel.text = GameController.Instance.DefaultPilotTitle;
		}

		protected override void update()
		{
			base.update();
			if (Screen.fullScreen != FullScreenToggle.isOn)
			{
				FullScreenToggle.onValueChanged.RemoveAllListeners();
				FullScreenToggle.isOn = Screen.fullScreen;
				FullScreenToggle.onValueChanged.AddListener(FullScreenToggleValueChanged);
			}
			if (resizeUi && RealTime.time > reizeUiTime)
			{
				GameController.Instance.CanvasHeight01 = 1f - InterfaceSizeSlider.value;
				GameController.Instance.ScreenScaler.ScaleScreens();
				resizeUi = false;
			}
		}

		protected override bool onNavigatingBack()
		{
			Save();
			GameController.Instance.ApplyAllPlayerPrefs();
			return base.onNavigatingBack();
		}

		private void CalibrateTiltButton_Activated()
		{
			GameController.Instance.TiltControlDefaultAccelerationX = Input.acceleration.x;
			UIController.Instance.ShowMessageBox("Tilt control is calibrated for current angle", MessageBoxButtons.Ok, null, MessageBoxIcon.Ok);
		}

		private void GraphicsQualitySliderValueChanged(float value)
		{
			RefreshGraphicsQualityText();
		}

		private void InterfaceSizeSlider_ValueChanged(float change)
		{
			resizeUi = true;
			reizeUiTime = RealTime.time + 1f;
		}

		private void MusicVolumeSlider_ValueChanged(float change)
		{
			if (GameController.Instance != null && GameController.Instance.MusicPlayer != null)
			{
				GameController.Instance.MusicPlayer.Volume = MusicVolumeSlider.value;
			}
		}

		private void CameraDragRotationSensitivitySliderValueChanged(float change)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.CameraDragRotateSensitivity = CameraDragRotationSensitivitySlider.value;
			}
		}

		private void MasterVolumeSlider_ValueChanged(float change)
		{
			AudioListener.volume = MasterVolumeSlider.value;
		}

		private void TiltSensitivitySlider_ValueChanged(float change)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.TiltSensitivityNorm = TiltSensitivitySlider.value;
			}
		}

		private void TouchTurnSensitivitySlider_ValueChanged(float change)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.TouchTurnRadiusNorm = 1f - TouchTurnSensitivitySlider.value;
			}
		}

		private void AutoCloseInGameMenuToggleValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.AutoCloseInGameMenu = value;
			}
		}

		private void ButtonSoundsToggle_ValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.PlayButtonSounds = value;
			}
		}

		private void MissileLockSoundToggleValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.PlayerOptions.Audio_MissileLockSound = value;
			}
		}

		private void LightingFXToggleValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.LightingFXEnabled = value;
			}
		}

		private void SpaceFogEnabledToggle_ValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.SpaceFogEnabled = value;
			}
		}

		private void RenderCloakedShipOutlinesEnabledToggle_ValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.RenderCloakedShipOutlinesEnabled = value;
			}
		}

		private void WormholeAnimationToggle_ValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.PlayerOptions.Video_WormholeAnimationEnabled = value;
			}
		}

		private void AutoTargetHostilesToggleValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.AutoTargetHostiles = value;
			}
		}

		private void PreferRespawnOnDeathToggleValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.RespawnOnDeath = value;
			}
		}

		private void PreferCameraToggle_ValueChanged(bool value)
		{
			if (GameController.Instance != null)
			{
				GameController.Instance.PreferCameraLock = value;
			}
		}

		private void TiltControlToggle_ValueChanged(bool value)
		{
			GameController.Instance.TiltControlEnabled = value;
		}

		private void ForceTouchControlEnabledToggle_ValueChanged(bool value)
		{
			GameController.Instance.ForceTouchInputEnabled = value;
		}

		private void AutoSaveOnDockingToggleValueChanged(bool value)
		{
			GameController.Instance.AutoSaveSettings.AutoSaveOnDocking = value;
		}

		private void AutoSaveOnWormholeExitToggleValueChanged(bool value)
		{
			GameController.Instance.AutoSaveSettings.AutoSaveOnWormholeExit = value;
		}

		private void AutoSaveAfterDurationToggleValueChanged(bool value)
		{
			if (value)
			{
				ResetEngineTimeOfLastAutoSave();
			}
			GameController.Instance.AutoSaveSettings.AutoSaveAfterDuration = value;
		}

		private void AutoHideShieldsToggleValueChanged(bool value)
		{
			GameController.Instance.PlayerOptions.UI_AutoHideShields = value;
		}

		private void ShowPointDefenceTurretsToggleValueChanged(bool value)
		{
			GameController.Instance.PlayerOptions.UI_ShowPointDefenceTurrets = value;
		}

		private void ScreenSpaceShieldsToggleValueChanged(bool value)
		{
			if (value)
			{
				GameController.Instance.PlayerOptions.UI_ScreenSpaceShields = true;
			}
		}

		private void General_AllowFireAtNeutralToggleValueChanged(bool value)
		{
			GameController.Instance.PlayerOptions.General_AllowFireAtNeutral = value;
		}

		private void General_General_AllowFireAtAlliedChanged(bool value)
		{
			GameController.Instance.PlayerOptions.General_AllowFireAtAllied = value;
		}

		private void WorldSpaceShieldsToggleValueChanged(bool value)
		{
			if (value)
			{
				GameController.Instance.PlayerOptions.UI_ScreenSpaceShields = false;
			}
		}

		private void ResetEngineTimeOfLastAutoSave()
		{
			if (EngineASX.Instance != null)
			{
				EngineASX.Instance.TimeOfLastAutoSave = EngineASX.Instance.ScenarioElapsedTime;
			}
		}

		private void TimedAutoSaveOptionValueChanged(bool toggleValue, int minutes)
		{
			ResetEngineTimeOfLastAutoSave();
			if (toggleValue)
			{
				GameController.Instance.AutoSaveSettings.AutoSaveAfterDurationMinutes = minutes;
			}
		}

		private void Save()
		{
			QualitySettings.SetQualityLevel((int)GraphicsQualitySlider.value, applyExpensiveChanges: false);
			GameController.Instance.CurrentQualityLevel = (int)GraphicsQualitySlider.value;
			Debug.Log("Saving Options...", this);
			GameController.Instance.SavePlayerPrefs();
		}
	}
}
