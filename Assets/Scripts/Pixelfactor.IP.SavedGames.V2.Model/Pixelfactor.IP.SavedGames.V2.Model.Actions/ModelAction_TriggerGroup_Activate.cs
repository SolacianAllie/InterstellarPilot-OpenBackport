using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.SavedGames.V2.Model.Triggers;

namespace Pixelfactor.IP.SavedGames.V2.Model.Actions
{
	public class ModelAction_TriggerGroup_Activate : ModelAction
	{
		public override ActionType Type => ActionType.TriggerGroup_Activate;

		public ModelTriggerGroup TriggerGroup { get; set; }
	}
}
