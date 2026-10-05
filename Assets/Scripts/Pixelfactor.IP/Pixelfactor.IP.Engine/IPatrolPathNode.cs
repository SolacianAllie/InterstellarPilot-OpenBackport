using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public interface IPatrolPathNode
	{
		Vector3 SectorPosition { get; set; }

		Sector Sector { get; }
	}
}
