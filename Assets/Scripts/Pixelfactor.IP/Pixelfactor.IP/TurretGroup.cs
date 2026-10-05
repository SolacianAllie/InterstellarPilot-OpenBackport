using System;
using System.Collections.Generic;
using Pixelfactor.IP.Engine;

namespace Pixelfactor.IP
{
	[Serializable]
	public class TurretGroup
	{
		public List<ActiveTurret> Turrets = new List<ActiveTurret>();
	}
}
