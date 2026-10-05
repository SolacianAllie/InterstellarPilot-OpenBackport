using System.Collections.Generic;
using Pixelfactor.Unity.Utils;

namespace Pixelfactor.IP.Engine.Dialog
{
	public class CompiledDialogEventHandler : IDialogEventHandler
	{
		public DialogEvent DialogEvent;

		public List<string> Messages { get; set; } = new List<string>();

		public string GetMessage(DialogRequestArguments? dialogRequestArguments)
		{
			return Messages.GetRandom();
		}
	}
}
