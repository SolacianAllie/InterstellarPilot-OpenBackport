using Pixelfactor.IP.Common.FleetOrders;
using Pixelfactor.IP.Engine.Factions;
using Pixelfactor.IP.Engine.Fleets.FleetOrders;

namespace Pixelfactor.IP.Engine.Fleets.ActiveOrders
{
	public class ActiveManualRearmOrder : ActiveRearmOrder
	{
		public ManualRearmOrder ManualRearmOrder;

		protected override void onInit()
		{
			base.onInit();
			if (CurrentRearmLocation == null)
			{
				CurrentRearmLocation = ManualRearmOrder.SpecificRearmLocation;
			}
		}

		protected override void OnRearmLocationNullOrInvalid()
		{
			base.OnRearmLocationNullOrInvalid();
			OnInvalid("Rearm location not valid");
		}

		public override void OnNpcUnableToDockAtNavpoint(NpcPilot npc, Unit dock, UnableToDockReason unableToDockReason)
		{
			switch (unableToDockReason)
			{
			case UnableToDockReason.Other:
				OnInvalid("Unable to dock");
				break;
			case UnableToDockReason.Refused:
				OnInvalid("Denied docking permission");
				break;
			default:
				base.OnNpcUnableToDockAtNavpoint(npc, dock, unableToDockReason);
				break;
			}
		}

		protected override string GetStatusTextInternal(Faction localFaction)
		{
			if (State == ActiveRearmFleetOrderState.Rearming)
			{
				if (rearmHasInsufficientCredits)
				{
					return "Insufficient credits";
				}
				return "Rearm In progress";
			}
			return base.GetStatusTextInternal(localFaction);
		}
	}
}
