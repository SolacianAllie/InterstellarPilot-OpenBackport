using System.Collections.Generic;
using System.Linq;
using OpenFrontier.IP.Engine;
using UnityEngine;
using UnityEngine.UI;

namespace OpenFrontier.IP.UI.Screens.MainMenu
{
	public class AutoSpectator : MonoBehaviour
	{
		private CameraSpectator cameraSpectator;

		private float lastSwitchTime;

		// Dwell time between camera swaps.
		private float maxSwitchTime = 18f;

		private WorldBase world;

		private Unit spectatingUnit;

		public float MinStateTimeUntilChange = 5f;

		// Target switches hard-snap by design (stock CameraSpectator), so
		// the snap is hidden behind a full-screen black UI fade. ALL
		// swaps are forced through RequestSwitch -> FadeSwitchRoutine:
		// FindAndSpectateShip may ONLY run at full black inside it.
		public float FadeOutDuration = 0.6f;

		public float FadeHoldDuration = 0.15f;

		public float FadeInDuration = 1f;

		private bool fadeInProgress;

		private Image fadeImage;

		private void Awake()
		{
			world = UnityObjectHelper.FindComponent<WorldBase>();
			world.Initialised += WorldInitialised;
		}

		private void WorldInitialised(WorldBase sender)
		{
			sender.Initialised -= WorldInitialised;
			cameraSpectator = sender.Engine.CameraSpectator;
			FindAndSpectateShip();
		}

		private void FindAndSpectateShip()
		{
			spectatingUnit = FindSpecateTarget();
			if (spectatingUnit != null)
			{
				world.Engine.ActiveSector = spectatingUnit.Sector;
				world.Engine.CameraStartSpectateUnit(spectatingUnit, allowFlyby: true, allowViewport: false, allowOrbit: true);
				world.Engine.OnCameraMoved();
			}
			else if (world.Engine.ActiveSector == null)
			{
				world.Engine.ActiveSector = world.Engine.Sectors.FirstOrDefault();
			}
		}

		// Open Frontier: pick RANDOMLY (FirstOrDefault on a stable
		// enumeration just alternated between the same two ships
		// forever), preferring ships that are DOING something - on a
		// trade run, mining, docking, in combat - over parked ones.
		private Unit FindSpecateTarget()
		{
			List<Unit> list = new List<Unit>();
			List<Unit> list2 = new List<Unit>();
			EngineASX.Instance.EnumerateUnitsWithPredicate(delegate(Unit unit)
			{
				if (IsBusy(unit))
				{
					list.Add(unit);
				}
				else
				{
					list2.Add(unit);
				}
			}, (Unit e) => e.IsValidAndNotDestroyed && (spectatingUnit == null || e.GetRootUnit() != spectatingUnit.GetRootUnit()) && e.UnitType == UnitType.Ship);
			List<Unit> list3 = ((list.Count > 0) ? list : list2);
			if (list3.Count <= 0)
			{
				return null;
			}
			return list3[UnityEngine.Random.Range(0, list3.Count)];
		}

		// "Doing something": not docked, and either working an active
		// fleet order (trade run, mine op, dock approach, patrol) or in
		// combat. Miners parked ON a rock still hold their mine order,
		// so stationary mining counts as busy.
		private static bool IsBusy(Unit unit)
		{
			if (unit.IsDocked)
			{
				return false;
			}
			NpcPilot npcPilot = unit.NpcPilot;
			if (npcPilot == null)
			{
				return false;
			}
			if (npcPilot.HasCombatTargetOrGroupInCombat)
			{
				return true;
			}
			Fleet fleet = npcPilot.Fleet;
			return fleet != null && fleet.ActiveOrder != null;
		}

		private void Update()
		{
			if (!EngineASX.LoadedAndReady || fadeInProgress)
			{
				return;
			}
			// Watchdog: a target that docked, died or left the sector
			// leaves the camera staring at empty space (or the cargo
			// crates it just dumped). Retarget quickly; the short
			// cooldown stops thrash while EVERYTHING is docked.
			if ((spectatingUnit == null || !spectatingUnit.IsValidAndNotDestroyed || spectatingUnit.IsDocked) && Time.time > lastSwitchTime + 2f)
			{
				RequestSwitch();
				return;
			}
			if (IsReadyToSwitchUnit())
			{
				RequestSwitch();
			}
		}

		// The ONLY way to swap the spectate target. The camera cannot
		// switch until the screen is fully black - no fade, no swap.
		private void RequestSwitch()
		{
			if (!fadeInProgress)
			{
				StartCoroutine(FadeSwitchRoutine());
			}
		}

		private System.Collections.IEnumerator FadeSwitchRoutine()
		{
			fadeInProgress = true;
			EnsureFadeCanvas();
			yield return FadeTo(1f, FadeOutDuration);
			Unit unit = spectatingUnit;
			FindAndSpectateShip();
			if (spectatingUnit != unit)
			{
				lastSwitchTime = Time.time;
			}
			// A beat at full black so the camera attaches unseen.
			float num = 0f;
			while (num < FadeHoldDuration)
			{
				num += Time.unscaledDeltaTime;
				yield return null;
			}
			yield return FadeTo(0f, FadeInDuration);
			fadeInProgress = false;
		}

		private System.Collections.IEnumerator FadeTo(float targetAlpha, float duration)
		{
			float a = fadeImage.color.a;
			for (float num = 0f; num < duration; num += Time.unscaledDeltaTime)
			{
				SetFadeAlpha(Mathf.Lerp(a, targetAlpha, num / duration));
				yield return null;
			}
			SetFadeAlpha(targetAlpha);
		}

		private void SetFadeAlpha(float alpha)
		{
			Color color = fadeImage.color;
			color.a = Mathf.Clamp01(alpha);
			fadeImage.color = color;
		}

		// The blackout layer: unlit full-screen black Image on an overlay
		// canvas, raycast-free so the menu stays clickable under it.
		// Created lazily (native UI calls are forbidden in initializers).
		private void EnsureFadeCanvas()
		{
			if (fadeImage != null)
			{
				return;
			}
			GameObject gameObject = new GameObject("SpectateFadeCanvas");
			gameObject.transform.SetParent(transform, worldPositionStays: false);
			Canvas canvas = gameObject.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvas.sortingOrder = 5000;
			GameObject gameObject2 = new GameObject("FadeImage");
			gameObject2.transform.SetParent(gameObject.transform, worldPositionStays: false);
			fadeImage = gameObject2.AddComponent<Image>();
			fadeImage.color = new Color(0f, 0f, 0f, 0f);
			fadeImage.raycastTarget = false;
			RectTransform rectTransform = fadeImage.rectTransform;
			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
		}

		private bool IsReadyToSwitchUnit()
		{
			if (Time.time > lastSwitchTime + maxSwitchTime)
			{
				return (double)Time.time - cameraSpectator.LastStateChangeTime > (double)MinStateTimeUntilChange;
			}
			return false;
		}
	}
}
