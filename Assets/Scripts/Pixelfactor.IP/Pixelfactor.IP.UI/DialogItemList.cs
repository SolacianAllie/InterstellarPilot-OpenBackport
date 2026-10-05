using Pixelfactor.IP.Engine;
using Pixelfactor.IP.Engine.Comms;
using UnityEngine;

namespace Pixelfactor.IP.UI
{
	public class DialogItemList : ScrollList<ICommsStageOption>
	{
		public DialogController DialogController;

		public Color ReadTextColor = Color.grey;

		public Color UnreadTextColor = Color.white;
	}
}
