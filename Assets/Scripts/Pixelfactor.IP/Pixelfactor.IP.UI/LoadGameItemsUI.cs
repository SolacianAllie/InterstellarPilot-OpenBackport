using System.Linq;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.SaveGame;
using Pixelfactor.IP.UI.Screens.LoadGame;

namespace Pixelfactor.IP.UI
{
	public class LoadGameItemsUI : ScrollList<EngineSaveGameHeader>
	{
		public LoadGameScreen LoadGameUI;

		protected override void OnRefreshing()
		{
			SetItems(from e in SaveGameUtilities.FindSavedGameHeaders(includeIncompatibleVersions: true)
				orderby e.TimeStamp descending
				select e);
		}
	}
}
