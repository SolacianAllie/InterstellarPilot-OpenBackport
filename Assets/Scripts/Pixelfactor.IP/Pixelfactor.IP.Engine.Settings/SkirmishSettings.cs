using System.Collections.Generic;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Settings
{
	public class SkirmishSettings : MonoBehaviour
	{
		public int MaxSkirmishTeams = 8;

		public int MaxSkirmishTeamShips = 12;

		public List<Color> DefaultTeamColours = new List<Color>();
	}
}
