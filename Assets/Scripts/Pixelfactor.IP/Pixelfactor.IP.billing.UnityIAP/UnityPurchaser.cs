using System;
using Pixelfactor.IP.UI;
using Pixelfactor.IP.UI.Screens;
using Pixelfactor.IP.UI.Screens.MessageBox;
using UnityEngine;
using UnityEngine.Purchasing;
using Object = UnityEngine.Object;

namespace Pixelfactor.IP.billing.UnityIAP
{
	public class UnityPurchaser : MonoBehaviour, IStoreListener, IPurchaser
	{
		private int? initializationFailureResult;

		private static IStoreController storeController;

		private static IExtensionProvider m_StoreExtensionProvider;

		public int? InitializationFailureResult
		{
			get
			{
				return initializationFailureResult;
			}
			private set
			{
				initializationFailureResult = value;
			}
		}

		public IPProduct[] Products { get; set; }

		public bool IsInitialised => IsInitialized();

		public event PurchaseMadeHandler PurchaseMade;

		public event PurchaserInitialisedHandler Initialised;

		public event PurchasesRestoredHandler PurchasesRestored;

		public event PurchaserInitialisationFailedHandler InitialisationFailed;

		public void Init()
		{
			if (storeController == null)
			{
				InitializePurchasing();
			}
		}

		public void InitializePurchasing()
		{
			if (Products == null)
			{
				if (LogWrapper.LogMsgs)
				{
					Debug.LogError("Products have not been set", this);
				}
			}
			else if (!IsInitialized())
			{
				ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
				IPProduct[] products = Products;
				foreach (IPProduct iPProduct in products)
				{
					configurationBuilder.AddProduct(iPProduct.Id, ProductType.NonConsumable);
				}
				UnityPurchasing.Initialize(this, configurationBuilder);
			}
		}

		public bool TryGetPriceText(string productId, out string price)
		{
			price = null;
			if (IsInitialized())
			{
				Product product = storeController.products.WithID(productId);
				if (product != null)
				{
					string localizedPriceString = product.metadata.localizedPriceString;
					if (localizedPriceString != "$0.01")
					{
						price = localizedPriceString;
						return true;
					}
					return false;
				}
				if (LogWrapper.LogMsgs)
				{
					Debug.LogError("Unknown product: " + productId);
				}
			}
			return false;
		}

		private bool IsInitialized()
		{
			if (storeController != null)
			{
				return m_StoreExtensionProvider != null;
			}
			return false;
		}

		public void BuyProductID(string productId)
		{
			if (IsInitialized())
			{
				Product product = storeController.products.WithID(productId);
				if (product == null)
				{
					return;
				}
				if (product.hasReceipt)
				{
					OnProcessedPurchase(productId);
				}
				else if (product != null)
				{
					if (LogWrapper.LogMsgs)
					{
						Debug.Log($"Purchasing product asychronously: '{product.definition.id}'");
					}
					storeController.InitiatePurchase(product);
				}
				else if (LogWrapper.LogMsgs)
				{
					Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
				}
			}
			else if (LogWrapper.LogMsgs)
			{
				Debug.Log("BuyProductID FAIL. Not initialized.");
			}
		}

		public void RestorePurchases()
		{
			if (!IsInitialized())
			{
				if (LogWrapper.LogMsgs)
				{
					Debug.Log("RestorePurchases FAIL. Not initialized.");
				}
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
			{
				if (LogWrapper.LogMsgs)
				{
					Debug.Log("RestorePurchases started ...");
				}
				m_StoreExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((bool result) =>
				{
					if (LogWrapper.LogMsgs)
					{
						Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
					}
					if (PurchasesRestored != null)
					{
						PurchasesRestored(this, result);
					}
				});
			}
			else if (LogWrapper.LogMsgs)
			{
				Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			initializationFailureResult = null;
			if (LogWrapper.LogMsgs)
			{
				Debug.Log("OnInitialized: PASS");
			}
			storeController = controller;
			m_StoreExtensionProvider = extensions;
			IPProduct[] products = Products;
			foreach (IPProduct iPProduct in products)
			{
				Product product = storeController.products.WithID(iPProduct.Id);
				if (product != null && product.hasReceipt)
				{
					Pixelfactor.IP.billing.Products.SetHasProduct(iPProduct.Id, hasProduct: true);
				}
			}
			if (Initialised != null)
			{
				Initialised(this);
			}
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
			Debug.LogError("Purchase Initializaion Failure:  Reason:" + error);
			initializationFailureResult = (int)error;
			if (InitialisationFailed != null)
			{
				InitialisationFailed(this);
			}
		}

		private bool IsValidProduct(string productId)
		{
			IPProduct[] products = Products;
			foreach (IPProduct iPProduct in products)
			{
				if (string.Equals(productId, iPProduct.Id, StringComparison.Ordinal))
				{
					return true;
				}
			}
			return false;
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
		{
			if (IsValidProduct(args.purchasedProduct.definition.id))
			{
				if (LogWrapper.LogMsgs)
				{
					Debug.Log($"ProcessPurchase: PASS. Product: '{args.purchasedProduct.definition.id}'");
				}
				string id = args.purchasedProduct.definition.id;
				OnProcessedPurchase(id);
			}
			else if (LogWrapper.LogMsgs)
			{
				Debug.Log($"ProcessPurchase: FAIL. Unrecognized product: '{args.purchasedProduct.definition.id}'");
			}
			return PurchaseProcessingResult.Complete;
		}

		private void OnProcessedPurchase(string productId)
		{
			Pixelfactor.IP.billing.Products.SetHasProduct(productId, hasProduct: true);
			NotifyPurchaseMade(productId);
		}

		private void NotifyPurchaseMade(string productId)
		{
			if (PurchaseMade != null)
			{
				PurchaseMade(this, productId);
			}
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			if (failureReason == PurchaseFailureReason.Unknown)
			{
				ShowUnableToProcessPurchase();
			}
			if (LogWrapper.LogMsgs)
			{
				Debug.Log($"OnPurchaseFailed: FAIL. Product: '{product.definition.storeSpecificId}', PurchaseFailureReason: {failureReason}");
			}
		}

		private static void ShowUnableToProcessPurchase()
		{
			string message = "Unable to process purchase";
			UIController.Instance.ShowMessageBox(message, MessageBoxButtons.Ok, null, MessageBoxIcon.Error);
		}

		public void Dispose()
		{
			UnityEngine.Object.Destroy(this);
		}
	}
}
