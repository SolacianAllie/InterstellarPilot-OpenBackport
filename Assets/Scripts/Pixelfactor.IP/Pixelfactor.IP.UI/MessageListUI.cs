using System.Collections.Generic;
using Pixelfactor.IP.Engine;
using Pixelfactor.IP.UI.Screens;

namespace Pixelfactor.IP.UI
{
	public class MessageListUI : ScrollList<PlayerActiveMessage>
	{
		public MessagesUI MessagesUI;

		protected override void OnRefreshing()
		{
			base.OnRefreshing();
			List<PlayerActiveMessage> displayableMessages = PlayerMessagesHelper.GetDisplayableMessages();
			displayableMessages.Sort(MessagesUI.ItemComparer);
			SetItems(displayableMessages);
		}
	}
}
