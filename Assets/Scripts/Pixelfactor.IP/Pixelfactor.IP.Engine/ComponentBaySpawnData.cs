using System;
using Pixelfactor.IP.Engine.UnitComponents;

namespace Pixelfactor.IP.Engine
{
	[Serializable]
	public class ComponentBaySpawnData
	{
		public float InitialCondition = 1f;

		public ComponentBay TargetBay;
	}
}
