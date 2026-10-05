using System;
using Pixelfactor.IP.IO;
using UnityEngine;

namespace Pixelfactor.IP.Engine.Core
{
	public class GameData : MonoBehaviour
	{
		public TextAsset BarTypeNamesTextAsset;

		[NonSerialized]
		public string[] BarTypeNames;

		public void Init()
		{
			BarTypeNames = IOHelper.ReadAndSplitTextLines(BarTypeNamesTextAsset);
		}
	}
}
