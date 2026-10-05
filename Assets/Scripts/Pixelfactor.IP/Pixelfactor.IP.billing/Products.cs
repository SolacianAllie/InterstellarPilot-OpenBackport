using Pixelfactor.IP.ZFrame;

namespace Pixelfactor.IP.billing
{
	public static class Products
	{
		public static bool IAPEnabled => false;

		public static bool HasProduct(string product)
		{
			return ZPlayerPrefs.GetInt(product, 0) == 1;
		}

		public static bool HasProduct(IPProduct product)
		{
			return HasProduct(product.Id);
		}

		public static void SetHasProduct(string product, bool hasProduct)
		{
			ZPlayerPrefs.SetInt(product, hasProduct ? 1 : 0);
		}
	}
}
