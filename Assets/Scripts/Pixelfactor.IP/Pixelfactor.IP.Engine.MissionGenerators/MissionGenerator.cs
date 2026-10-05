using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.IP.Engine.MissionSpecs;
using UnityEngine;

namespace Pixelfactor.IP.Engine.MissionGenerators
{
	public abstract class MissionGenerator : MonoBehaviour
	{
		public abstract MissionManagerMissionType MissionType { get; }

		public MissionSpec GenerateMissionSpec(Unit missionLocationUnit, Faction faction, EngineASX engine)
		{
			return generateMissionSpec(missionLocationUnit, faction, engine);
		}

		protected abstract MissionSpec generateMissionSpec(Unit missionLocationUnit, Faction faction, EngineASX engine);

		public virtual bool CanGenerateMission(Unit missionLocationUnit, Faction missionLocationFaction)
		{
			return true;
		}
	}
}
