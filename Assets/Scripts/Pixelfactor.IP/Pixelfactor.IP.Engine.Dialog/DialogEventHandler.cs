using System.Collections.Generic;
using Pixelfactor.Unity.Utils;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Dialog
{
	public class DialogEventHandler : MonoBehaviour, IDialogEventHandler
	{
		public DialogEvent Event;

		public List<string> Messages;

		public string GetMessage(DialogRequestArguments? dialogRequestArguments)
		{
			return Messages.GetRandom();
		}
	}
}
