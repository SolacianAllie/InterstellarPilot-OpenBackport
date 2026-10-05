using System.Collections.Generic;
using Pixelfactor.IP.Engine.Pathfinding;

namespace Pixelfactor.IP.Engine.Factions.Intel
{
	public class UniversePath
	{
		public int Jumps;

		public List<UniversePathNode> Nodes = new List<UniversePathNode>(16);
	}
}
