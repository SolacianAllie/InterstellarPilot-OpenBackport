using Pixelfactor.IP.Common.Triggers;
using Pixelfactor.IP.SavedGames.V2.Model.Jobs;

namespace Pixelfactor.IP.SavedGames.V2.Model.Actions
{
	public class ModelAction_Mission_Activate : ModelAction
	{
		public override ActionType Type => ActionType.Mission_Activate;

		public ModelMission Mission { get; set; }
	}
}
