using Pixelfactor.IP.Common;

namespace Pixelfactor.IP.SavedGames.V2.Model.Jobs
{
	public class ModelJob
	{
		public int Id { get; set; }

		public double ExpiryTime { get; set; }

		public int RewardCredits { get; set; }

		public int ProfitCredits { get; set; }

		public ModelFaction Faction { get; set; }

		public ModelUnit Unit { get; set; }

		public virtual JobType JobType => JobType.None;
	}
}
