using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.EngineActions;
using Pixelfactor.IP.Testing.Spawning;
using UnityEngine;

namespace Pixelfactor.IP.Assets.Scripts.Engine.EngineActions
{
	public class Action_Spawn_BanditHorde : EngineAction
	{
		public Sector TargetSector;

		public Vector3 TargetSectorPosition = Vector3.zero;

		public override void Execute()
		{
			base.Execute();
			if (TargetSector != null)
			{
				SpawnUtils.SpawnBanditHordesAtSectorPosition(TargetSector, TargetSectorPosition);
			}
		}
	}
}
