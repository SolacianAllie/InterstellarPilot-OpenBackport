using System;
using Pixelfactor.IP.Common;
using Pixelfactor.IP.SavedGames.V2.Model.Jobs;
using Pixelfactor.IP.SavedGames.V2.Model.Jobs.JobTypes;

namespace Pixelfactor.IP.SavedGames.V2.Model
{
	public static class CreateJobFromJobType
	{
		public static ModelJob CreateJob(JobType jobType)
		{
			return jobType switch
			{
				JobType.DestroyGroup => (ModelJob)new ModelDestroyFleetJob(), 
				JobType.Courier => new ModelCourierJob(), 
				JobType.DeliverShip => new ModelDeliverShipJob(), 
				JobType.Breakdown => new ModelBreakdownJob(), 
				_ => throw new NotImplementedException($"Unknown job type {(int)jobType}"), 
			};
		}
	}
}
