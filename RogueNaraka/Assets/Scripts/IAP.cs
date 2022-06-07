using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAP : MonoBehaviour, IStoreListener
{
   

    private static IStoreController m_StoreController;
    private static IExtensionProvider m_StoreExtensionProvider;

    public string remove_ads = "remove_ads";
    public string coin_1 = "coin_1";
    public string coin_2 = "coin_2";
    public string coin_3 = "coin_3";


    void Start()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }

    }

    public void InitializePurchasing()
    {
        if (IsInitialized())
        {
            return;
        }

        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        builder.AddProduct(remove_ads, ProductType.NonConsumable);
        builder.AddProduct(coin_1, ProductType.Consumable);
        builder.AddProduct(coin_2, ProductType.Consumable);
        builder.AddProduct(coin_3, ProductType.Consumable);


        UnityPurchasing.Initialize(this, builder);
    }

    public void BuyRemoveAds()
    {
        BuyProductID(remove_ads);
    }

    public void BuyCoin(int num){
        switch(num){
            case 1: BuyProductID(coin_1); break;
            case 2: BuyProductID(coin_2); break;
            case 3: BuyProductID(coin_3); break;
        }
    }

    

    void BuyProductID(string productId)
    {
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
                
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                LobbyManager.Instance.SetAlert("Purchase Failed : Not available for purchase");
            }
        }
        else
        {
            Debug.Log("BuyProductID FAIL. Not initialized.");
            LobbyManager.Instance.SetAlert("Purchase Failed : Not initialized.");
        }
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
         if (String.Equals(args.purchasedProduct.definition.id, remove_ads, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));

            
            if (PlayerPrefs.GetInt("isRemoveAds") == 0)
            {
                LobbyManager.Instance.PurchaseRemoveAds();

            }
            
        }
        else if (String.Equals(args.purchasedProduct.definition.id, coin_1, StringComparison.Ordinal))
        {
            MoneyManager.instance.AddSoul(400);
            LobbyManager.Instance.IAPUIUpdate();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, coin_2, StringComparison.Ordinal))
        {
            MoneyManager.instance.AddSoul(2400);
            LobbyManager.Instance.IAPUIUpdate();
        }
        else if (String.Equals(args.purchasedProduct.definition.id, coin_3, StringComparison.Ordinal))
        {
            MoneyManager.instance.AddSoul(5500);
            LobbyManager.Instance.IAPUIUpdate();
        }

        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            LobbyManager.Instance.SetAlert("Purchase Failed");
        }

        return PurchaseProcessingResult.Complete;
    }

    public void RestorePurchases() 
    {
        if (!IsInitialized())
        {
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer) 
        {
            Debug.Log("RestorePurchases started ...");

            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();

            apple.RestoreTransactions((result) =>
            {
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        else
        {
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }



    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("OnInitialized: PASS");
        m_StoreController = controller;
        m_StoreExtensionProvider = extensions;
    }

    private bool IsInitialized()
    {
        return m_StoreController != null && m_StoreExtensionProvider != null;
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
    }



}