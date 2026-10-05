using Pixelfactor.IP.Engine.UnitComponents;

namespace Pixelfactor.IP.Engine.CustomUnitVariants
{
	public class CustomUnitVariantBayMod
	{
		public int BayId { get; set; }

		public ComponentClass ComponentClass { get; set; }

		public CustomUnitVariantBayMod Clone()
		{
			return new CustomUnitVariantBayMod
			{
				BayId = BayId,
				ComponentClass = ComponentClass
			};
		}
	}
}
