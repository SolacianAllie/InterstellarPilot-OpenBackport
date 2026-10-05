using Pixelfactor.IP.Engine.Factions;

namespace Pixelfactor.IP.Engine.Comms
{
	public interface ICommsHandler
	{
		Faction OwnerFaction { get; }

		Person OwnerPilot { get; }

		bool IsEnabled { get; set; }

		ICommsStage GetStage();

		void Hide();

		void Show();
	}
}
