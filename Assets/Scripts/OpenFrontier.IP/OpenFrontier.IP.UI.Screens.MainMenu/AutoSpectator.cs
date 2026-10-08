using System.Collections.Generic;
using System.Linq;
using OpenFrontier.IP.Engine;
using UnityEngine;

namespace OpenFrontier.IP.UI.Screens.MainMenu
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
