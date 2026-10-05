using Pixelfactor.IP.Engine;
using Pixelfactor.Unity.Utils;

namespace Pixelfactor.IP.Scenarios
{
	public static class FactionObjectiveAssigner
	{
		private class WeightedUnit : IWeighted
		{
			public float Weight { get; set; }

			public Unit Unit { get; set; }
		}
	}
}
