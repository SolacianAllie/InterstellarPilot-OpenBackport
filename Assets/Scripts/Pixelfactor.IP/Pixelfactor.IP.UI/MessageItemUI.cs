using Pixelfactor.IP.Engine;
using TMPro;

namespace Pixelfactor.IP.UI
{
	public class MessageItemUI : ScrollListItem<PlayerActiveMessage>
	{
		public TextMeshProUGUI FromLabel;

		public TextMeshProUGUI SubjectText;

		public TextMeshProUGUI DateLabel;

		public MessagesUI MessagesUI => ((MessageListUI)ParentList).MessagesUI;

		public override void Refresh()
		{
			base.Refresh();
			if (Item != null)
			{
				DateLabel.text = EngineASX.Instance.DateTimeUtils.GetFormattedEngineTimeStampFromRealElapsedSeconds(Item.EngineTimeStamp);
				FromLabel.text = Item.GetFriendlyFromText();
				SubjectText.text = Item.GetFriendlySubjectText();
				SubjectText.fontStyle = ((!Item.Opened) ? FontStyles.Bold : FontStyles.Normal);
				DateLabel.fontStyle = SubjectText.fontStyle;
			}
		}

		protected override void OnButtonClick()
		{
			base.OnButtonClick();
			if (MessagesUI != null)
			{
				MessageUI.ShowMessage = Item;
				UIController.Instance.ScreenNavigator.NavigateToScreen<MessageUI>();
			}
		}
	}
}
