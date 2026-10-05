using System.Collections.Generic;
using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.SavedGames.V2.Model.Jobs;

namespace Pixelfactor.IP.SavedGames.V2.Model.Actions
{
	public class ModelAction_Mission_ActivateObjective : ModelAction
	{
		public override ActionType Type => ActionType.Mission_ActivateObjective;

		public List<ModelMissionObjective> Objectives { get; set; } = new List<ModelMissionObjective>();
	}
}
