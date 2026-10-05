using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public class MissionStage : MonoBehaviour
	{
		public bool CompletesMission;

		public string JournalEntry;

		public bool MissionSuccess = true;

		public void AutoNameGameObject()
		{
			name = "MissionStage";
		}
	}
}
