using Pixelfactor.IP.Engine.UnitComponents;
using UnityEngine;

namespace Pixelfactor.IP.Engine
{
	public class UnitComponentTrader : MonoBehaviour
	{
		public bool SellsSpecificComponents;

		public ComponentClass[] AvailableComponentClasses;

		public UnitComponentHolder UnitComponents { get; private set; }

		public void Init()
		{
			UnitComponents = GetComponent<UnitComponentHolder>();
		}
	}
}
