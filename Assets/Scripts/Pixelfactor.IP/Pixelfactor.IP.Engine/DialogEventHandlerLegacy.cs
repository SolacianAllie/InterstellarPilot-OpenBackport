using System;
using System.Collections.Generic;
using Pixelfactor.IP.Engine.Dialog;
using Pixelfactor.Unity.Utils;

namespace Pixelfactor.IP.Engine
{
	[Serializable]
	public class DialogEventHandlerLegacy : IDialogEventHandler
	{
		public DialogEvent Event;

		public List<string> Messages;

		public string GetMessage(DialogRequestArguments? dialogRequestArguments)
		{
			return Messages.GetRandom();
		}
	}
}
