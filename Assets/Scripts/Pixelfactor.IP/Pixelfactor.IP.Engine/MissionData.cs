using Pixelfactor.IP.Engine.MissionObjectives;
using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public class MissionData : MonoBehaviour
	{
		public bool BroadcastMessages = true;

		public bool IsPrimary;

		public Mission MissionPrefab;

		public MissionObjectiveData[] Objectives;

		public MissionStage[] Stages;

		public string Title;

		public int UniqueId;
	}
}
