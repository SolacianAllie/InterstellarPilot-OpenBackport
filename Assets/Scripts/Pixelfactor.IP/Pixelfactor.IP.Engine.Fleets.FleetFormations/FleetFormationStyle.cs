using System.Collections.Generic;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Fleets.FleetFormations
{
	public class FleetFormationStyle : MonoBehaviour
	{
		public int UniqueId;

		public string Name = "Formation";

		public Sprite Sprite;

		public float ScaleFudge = 1f;

		public List<FleetFormationDesign> Formations { get; set; }
	}
}
