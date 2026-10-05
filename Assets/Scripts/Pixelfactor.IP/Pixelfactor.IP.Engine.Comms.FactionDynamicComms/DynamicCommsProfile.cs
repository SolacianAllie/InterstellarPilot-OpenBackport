using System.Collections.Generic;
using Pixelfactor.IP.Common.Factions;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Comms.FactionDynamicComms
{
	public class DynamicCommsProfile : MonoBehaviour
	{
		public Neutrality Neutrality;

		public DynamicCommsProfileOpinion Opinion;

		public List<string> Messages = new List<string>();
	}
}
