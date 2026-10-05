using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.Engine.Triggers;
using UnityEngine;

namespace Pixelfactor.IP.Engine.EngineActions
{
	public class Action_TriggerGroup_Activate : EngineAction
	{
		public bool Active = true;

		public TriggerGroup Target;

		public override ActionType Type => ActionType.TriggerGroup_Activate;

		public override void Execute()
		{
			base.Execute();
			if (Target != null)
			{
				Target.gameObject.SetActive(value: true);
			}
			else
			{
				Debug.LogError($"{this}: No target is defined", this);
			}
		}
	}
}
