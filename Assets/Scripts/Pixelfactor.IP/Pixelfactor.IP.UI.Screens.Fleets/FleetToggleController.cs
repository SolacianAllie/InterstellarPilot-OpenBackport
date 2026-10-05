using System.Collections.Generic;
using Pixelfactor.IP.Engine;
using UnityEngine;
using UnityEngine.UI;

namespace Pixelfactor.IP.UI.Screens.Fleets
{
	public class FleetToggleController : MonoBehaviour
	{
		public CanvasGroup ToggleListCanvasGroup;

		public Unit OrderedUnit;

		private List<Toggle> fleetToggles = new List<Toggle>();

		public Toggle UnitNoFleetToggle;

		public Transform UnitFleetContainer;

		public Toggle UnitFleetTogglePrefab;
	}
}
