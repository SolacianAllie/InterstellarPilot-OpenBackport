using Pixelfactor.IP.Engine.Fleets.FleetOrders;

namespace Pixelfactor.IP.Engine.AI.ActiveOrders
{
	public class ActiveManualRepairFleetOrder : ActiveRepairFleetOrder
	{
		public ManualRepairFleetOrder ManualRepairGroupObjective;

		protected override void onInit()
		{
			base.onInit();
			if (CurrentRepairLocation == null)
			{
				CurrentRepairLocation = ManualRepairGroupObjective.SpecificRepairLocation;
			}
		}

		protected override void OnRepairLocationNullOrInvalid()
		{
			base.OnRepairLocationNullOrInvalid();
			OnInvalid("Repair location not valid");
		}
	}
}
