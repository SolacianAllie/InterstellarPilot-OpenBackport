using System.Collections.Generic;

namespace Pixelfactor.IP.Engine.WorldGeneration.Models
{
	public class WorldBlueprint
	{
		public List<SectorBlueprint> SectorNodes { get; set; } = new List<SectorBlueprint>(16);
	}
}
