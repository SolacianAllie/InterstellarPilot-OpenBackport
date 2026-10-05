using System.Collections.Generic;
using Pixelfactor.IP.Engine.UnitComponents;

namespace Pixelfactor.IP.Engine.CompatibleComponents
{
	public class BayCompatibleComponents
	{
		private List<ComponentClass> components = new List<ComponentClass>(10);

		public List<ComponentClass> ComponentClasses => components;
	}
}
