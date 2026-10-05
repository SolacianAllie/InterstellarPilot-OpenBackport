using UnityEngine;

namespace Pixelfactor.IP.Engine.WorldGeneration.Models
{
	public class SectorConnection
	{
		public SectorBlueprint TargetSector;

		public Vector3 GetDirection(SectorBlueprint node)
		{
			return Vector3.Normalize(TargetSector.Position - node.Position);
		}

		public SectorConnection(SectorBlueprint target)
		{
			TargetSector = target;
		}
	}
}
