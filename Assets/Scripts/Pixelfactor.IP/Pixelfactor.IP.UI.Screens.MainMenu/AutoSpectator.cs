using System.Collections.Generic;
using System.Linq;
using Pixelfactor.IP.Engine;
using UnityEngine;

namespace Pixelfactor.IP.UI.Screens.MainMenu
{
	public class AutoSpectator : MonoBehaviour
	{
		private CameraSpectator cameraSpectator;

		private float lastSwitchTime;

		private float maxSwitchTime = 10f;

		private WorldBase world;

		private Unit spectatingUnit;

		public float MinStateTimeUntilChange = 5f;

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

		private Unit FindSpecateTarget()
		{
			List<Unit> list = new List<Unit>();
			EngineASX.Instance.EnumerateUnitsWithPredicate((Unit unit) =>
			{
				list.Add(unit);
			}, (Unit e) => e.IsValidAndNotDestroyed && (spectatingUnit == null || e.GetRootUnit() != spectatingUnit.GetRootUnit()) && e.UnitType == UnitType.Ship);
			return list.FirstOrDefault();
		}

		private void Update()
		{
			if (EngineASX.LoadedAndReady && IsReadyToSwitchUnit())
			{
				Unit unit = spectatingUnit;
				FindAndSpectateShip();
				if (spectatingUnit != unit)
				{
					lastSwitchTime = Time.time;
				}
			}
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
