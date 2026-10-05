using UnityEngine;

namespace Pixelfactor.IP.Engine.Npcs
{
	public class NpcPilotBehaviourMineDropSettings : MonoBehaviour
	{
		public float TargetDistanceMultiplierUpper = 1f;

		public float TargetDistanceMultiplierLower = 1f;

		public float AvoidDamageDistanceMultiplierLower = 0.75f;

		public float AvoidDamageDistanceMultiplierUpper = 1f;

		public bool AIActivePilotMineCheckFriendlies = true;

		public float AIActivePilotMineDropMinFriendlyDistMultiplier = 3f;
	}
}
