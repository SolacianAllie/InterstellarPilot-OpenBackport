using System.Collections.Generic;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.IP.Engine.Fleets.ActiveOrders;

namespace Pixelfactor.IP.UI.Screens.NewFleetOrder
{
	public class NewOrderTarget
	{
		private bool anyUnitHasMiningEquipment;

		private bool anyFleetHasHomeBase;

		private HashSet<int> allOrderedUnitIds = new HashSet<int>(8);

		private List<Unit> allUnits = new List<Unit>(8);

		private List<Unit> orderedUnits = new List<Unit>();

		private List<Fleet> orderedFleets = new List<Fleet>();

		private List<Unit> unitsWithoutFleet = new List<Unit>();

		private List<Sector> sectors = new List<Sector>();

		private bool allUnitsMobile = true;

		private bool allUnitsCanDock = true;

		private bool anyUnitCanTransportPassengers;

		private bool anyUnitArmed;

		private bool anyUnitIsDocked;

		private bool anyUnitHasTractorBeam;

		private Faction faction;

		public bool AnyUnitHasTractorBeam => anyUnitHasTractorBeam;

		public bool AnyUnitHasMiningEquipment => anyUnitHasMiningEquipment;

		public bool AnyUnitCanTransportPassengers => anyUnitCanTransportPassengers;

		public bool AnyUnitDocked => anyUnitIsDocked;

		public bool AnyFleetHasHomeBase => anyFleetHasHomeBase;

		public bool AnyUnitArmed => anyUnitArmed;

		public bool AllUnitsMobile => allUnitsMobile;

		public bool AllUnitsCanDock => allUnitsCanDock;

		public IEnumerable<Fleet> OrderedFleets => orderedFleets;

		public IEnumerable<Unit> OrderedUnits => orderedUnits;

		public IEnumerable<Unit> UnitsWithoutFleet => unitsWithoutFleet;

		public IEnumerable<Unit> AllUnits => allUnits;

		public bool IsSingleFleet
		{
			get
			{
				if (orderedFleets.Count == 1)
				{
					if (orderedUnits.Count != 0)
					{
						return unitsWithoutFleet.Count == 0;
					}
					return true;
				}
				return false;
			}
		}

		public bool IsSingleUnitWithoutFleet
		{
			get
			{
				if (orderedFleets.Count == 0)
				{
					return orderedUnits.Count == 1;
				}
				return false;
			}
		}

		public Unit SingleUnitWithoutFleet
		{
			get
			{
				if (IsSingleUnitWithoutFleet)
				{
					return orderedUnits[0];
				}
				return null;
			}
		}

		public Fleet SingleFleet
		{
			get
			{
				if (IsSingleFleet)
				{
					return orderedFleets[0];
				}
				return null;
			}
		}

		public bool HasMultipleFleets => orderedFleets.Count > 1;

		public bool MixedUnitsAndFleets
		{
			get
			{
				if (orderedFleets.Count > 0)
				{
					return unitsWithoutFleet.Count > 0;
				}
				return false;
			}
		}

		public Sector SingleSector
		{
			get
			{
				if (sectors.Count == 1)
				{
					return sectors[0];
				}
				return null;
			}
		}

		public List<Sector> Sectors => sectors;

		public bool IsValid => allOrderedUnitIds.Count > 0;

		public Faction Faction => faction;

		public static NewOrderTarget CreateFromUnits(IEnumerable<Unit> units)
		{
			NewOrderTarget newOrderTarget = new NewOrderTarget();
			foreach (Unit unit in units)
			{
				newOrderTarget.orderedUnits.Add(unit);
				Fleet fleet = unit.GetFleet();
				if (fleet != null)
				{
					if (!newOrderTarget.orderedFleets.Contains(fleet))
					{
						newOrderTarget.orderedFleets.Add(fleet);
					}
				}
				else
				{
					newOrderTarget.unitsWithoutFleet.Add(unit);
				}
			}
			newOrderTarget.faction = newOrderTarget.orderedUnits[0].Faction;
			newOrderTarget.CompileAllOrderedUnits();
			return newOrderTarget;
		}

		public static NewOrderTarget CreateFromFleets(Fleet fleet)
		{
			NewOrderTarget newOrderTarget = new NewOrderTarget
			{
				orderedFleets = { fleet }
			};
			newOrderTarget.faction = newOrderTarget.orderedFleets[0].Faction;
			newOrderTarget.CompileAllOrderedUnits();
			return newOrderTarget;
		}

		public static NewOrderTarget CreateFromFleets(IEnumerable<Fleet> fleets)
		{
			NewOrderTarget newOrderTarget = new NewOrderTarget();
			foreach (Fleet fleet in fleets)
			{
				newOrderTarget.orderedFleets.Add(fleet);
			}
			newOrderTarget.faction = newOrderTarget.orderedFleets[0].Faction;
			newOrderTarget.CompileAllOrderedUnits();
			return newOrderTarget;
		}

		private void CompileAllOrderedUnits()
		{
			anyUnitCanTransportPassengers = false;
			sectors.Clear();
			allUnitsMobile = true;
			anyUnitArmed = false;
			allUnitsCanDock = true;
			anyUnitIsDocked = false;
			anyFleetHasHomeBase = false;
			anyUnitHasMiningEquipment = false;
			anyUnitHasTractorBeam = false;
			allUnits.Clear();
			foreach (Unit orderedUnit in orderedUnits)
			{
				if (!allOrderedUnitIds.Contains(orderedUnit.UniqueId))
				{
					allOrderedUnitIds.Add(orderedUnit.UniqueId);
				}
				if (orderedUnit.Sector != null && !sectors.Contains(orderedUnit.Sector))
				{
					sectors.Add(orderedUnit.Sector);
				}
				allUnits.Add(orderedUnit);
			}
			foreach (Fleet orderedFleet in orderedFleets)
			{
				if (orderedFleet.IsHomeBaseValid)
				{
					anyFleetHasHomeBase = true;
				}
				foreach (UnitComponentHolder ship in orderedFleet.Ships)
				{
					if (ship != null)
					{
						Unit unit = ship.Unit;
						if (!allOrderedUnitIds.Contains(ship.Unit.UniqueId))
						{
							allOrderedUnitIds.Add(ship.Unit.UniqueId);
						}
						allUnits.Add(unit);
					}
				}
				if (orderedFleet.Sector != null && !sectors.Contains(orderedFleet.Sector))
				{
					sectors.Add(orderedFleet.Sector);
				}
			}
			foreach (Unit allUnit in allUnits)
			{
				if (!allUnit.IsMobile)
				{
					allUnitsMobile = false;
				}
				if (allUnit.IsArmed)
				{
					anyUnitArmed = true;
				}
				if (!allUnit.CanDock)
				{
					allUnitsCanDock = false;
				}
				if (allUnit.Components.PassengerCapacity > 0)
				{
					anyUnitCanTransportPassengers = true;
				}
				if (allUnit.IsDocked)
				{
					anyUnitIsDocked = true;
				}
				if (!anyUnitHasMiningEquipment && allUnit.HasMiningLaser() && allUnit.HasTractorBeam())
				{
					anyUnitHasMiningEquipment = true;
				}
				if (!anyUnitHasTractorBeam && allUnit.HasTractorBeam())
				{
					anyUnitHasTractorBeam = true;
				}
			}
		}

		public bool IsUnitOrdered(Unit unit)
		{
			if (unit != null)
			{
				return allOrderedUnitIds.Contains(unit.UniqueId);
			}
			return false;
		}

		public bool IsFleetOrdered(Fleet fleet)
		{
			return orderedFleets.Contains(fleet);
		}

		public IEnumerable<Fleet> CreateAndReturnFleets()
		{
			List<Fleet> list = new List<Fleet>();
			list.AddRange(orderedFleets);
			foreach (Unit item in unitsWithoutFleet)
			{
				Fleet fleet = item.GetFleet();
				if (fleet == null)
				{
					fleet = OrdersHelper.FindOrCreateNpcAndFleetAndPilotShip(item);
					list.Add(fleet);
				}
			}
			return list;
		}

		public bool AnyUnitNeedsRepair(float hullConditionThreshold, float componentsConditionThreshold, float shieldConditionThreshold)
		{
			foreach (Unit allUnit in allUnits)
			{
				if (allUnit != null && ActiveWaitForAutoRepairOrder.DoesUnitNeedRepair(allUnit, hullConditionThreshold, componentsConditionThreshold, shieldConditionThreshold))
				{
					return true;
				}
			}
			return false;
		}

		public List<CargoClass> GetAllCargoTypes()
		{
			List<CargoClass> list = new List<CargoClass>();
			foreach (Unit allUnit in allUnits)
			{
				if (!(allUnit.CargoBayComponent != null))
				{
					continue;
				}
				foreach (CargoClass cargoClass in allUnit.CargoBayComponent.CargoClasses)
				{
					if (!cargoClass.IsReserved && !list.Contains(cargoClass))
					{
						list.Add(cargoClass);
					}
				}
			}
			return list;
		}
	}
}
