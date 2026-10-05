using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.SavedGames.V2.Model.Jobs;

namespace Pixelfactor.IP.SavedGames.V2.Model.Actions
{
	public class ModelAction_Mission_ChangeStage : ModelAction
	{
		public override ActionType Type => ActionType.Mission_ChangeStage;

		public ModelMissionStage Stage { get; set; }
	}
}
