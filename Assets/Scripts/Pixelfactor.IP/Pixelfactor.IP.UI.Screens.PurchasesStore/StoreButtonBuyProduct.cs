using Pixelfactor.IP.Engine;
using Pixelfactor.IP.billing;
using UnityEngine;

namespace Pixelfactor.IP.UI.Screens.PurchasesStore
{
	public class StoreButtonBuyProduct : MonoBehaviour
	{
		public IPProduct Product;

		private void OnClick()
		{
			GameController.Instance.PurchaseBridge.Purchaser.BuyProductID(Product.Id);
		}
	}
}
