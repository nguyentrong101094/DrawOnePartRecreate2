﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
#if UNITY_ANDROID
using Google.Play.Billing;
#endif
using UnityEngine.Purchasing.Security;
using System.Linq;

public class InAppPurchaseHelper : MonoBehaviour, IStoreListener
{
    private static IStoreController m_StoreController;          // The Unity Purchasing system.
    private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.

    public delegate void PurchaseCompleteDelegate(bool success, PurchaseProcessingResult result, string productID);
    PurchaseCompleteDelegate onPurchaseComplete;

    // Product identifiers for all products capable of being purchased: 
    // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
    // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
    // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

    // General product identifiers for the consumable, non-consumable, and subscription products.
    // Use these handles in the code to reference which product to purchase. Also use these values 
    // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
    // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
    // specific mapping to Unity Purchasing's AddProduct, below.
    public static string productIDDiamond1 = "diamond_01";
    const int productDiamond1Payout = 200;

    public static string kProductIDNonConsumable = "test2";
    public static string kProductIDSubscription = "testsubscription";

    // Apple App Store-specific product identifier for the subscription product.
    private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

    // Google Play Store-specific product identifier subscription product.
    private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";


    public static InAppPurchaseHelper instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // If we haven't set up the Unity Purchasing reference
        if (m_StoreController == null)
        {
            // Begin to configure our connection to Purchasing
            InitializePurchasing();
        }

        //add no ads listener earlier: because initiating IAP take some time so splash ads was shown before no ads listener could be added in IAP initiation process
        IAPProcessor.SetupNoAds();
    }

    public void InitializePurchasing()
    {
        // If we have already connected to Purchasing ...
        if (IsInitialized())
        {
            // ... we are done here.
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE
        // Create a builder, first passing in a suite of Unity provided stores.
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
#elif UNITY_ANDROID
        // Create a builder using the GooglePlayStoreModule.
        var builder =
            ConfigurationBuilder.Instance(Google.Play.Billing.GooglePlayStoreModule.Instance());
#endif

        /*var storeModule = StandardPurchasingModule.Instance();
        if (Application.platform == RuntimePlatform.Android)
        {
            storeModule = Google.Play.Billing.GooglePlayStoreModule.Instance() as StandardPurchasingModule;
        }
        var builder = ConfigurationBuilder.Instance(storeModule);*/

        // Add a product to sell / restore by way of its identifier, associating the general identifier
        // with its store-specific identifiers.

        IAPProductData[] products = Resources.LoadAll(IAPProcessor.dataFolder, typeof(IAPProductData)).Cast<IAPProductData>().ToArray();
        foreach (var item in products)
        {
            builder.AddProduct(item.ProductId, item.productType);
        }

        // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
        // if the Product ID was configured differently between Apple and Google stores. Also note that
        // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
        // must only be referenced here. 
        /*builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
            { kProductNameAppleSubscription, AppleAppStore.Name },
            { kProductNameGooglePlaySubscription, GooglePlay.Name },
        });*/

        // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
        // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
    }

    private bool IsInitialized()
    {
        // Only say we are initialized if both the Purchasing references are set.
        bool value = m_StoreController != null && m_StoreExtensionProvider != null;
        if (!value) Debug.Log("IAP Helper not initialized");
        return value;
    }

    //Example code
    void BuyProductID(string productId)
    {
        // If Purchasing has been initialized ...
        if (IsInitialized())
        {
            // ... look up the Product reference with the general product identifier and the Purchasing 
            // system's products collection.
            Product product = m_StoreController.products.WithID(productId);

            // If the look up found a product for this device's store and that product is ready to be sold ... 
            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                // asynchronously.
                m_StoreController.InitiatePurchase(product);
            }
            // Otherwise ...
            else
            {
                // ... report the product look-up failure situation  
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        // Otherwise ...
        else
        {
            // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
            // retrying initiailization.
            Debug.Log("BuyProductID FAIL. Not initialized.");
        }
    }

    /// <summary>
    /// Inititate purchase process
    /// </summary>
    /// <param name="productId">Product ID, a ProductData file with matching name in Resources/Product Data should exists</param>
    /// <param name="purchaseCompleteDelegate">Callback on purchase complete, callback should display a message dialog displaying purchase result</param>
    public void BuyProduct(string productId, PurchaseCompleteDelegate purchaseCompleteDelegate)
    {
        // Buy the product using its general identifier. Expect a response either 
        // through ProcessPurchase or OnPurchaseFailed asynchronously.
        if (IsInitialized())
        {
            Product product = m_StoreController.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                onPurchaseComplete = purchaseCompleteDelegate;
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                m_StoreController.InitiatePurchase(product);
            }
            else
            {
                LogError($"BuyProductID: {productId} FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }
        else
        {
            LogError($"BuyProductID {productId} FAIL. Not initialized.");
        }
    }

    public Product GetProduct(string productId)
    {
        Product product = null;
        if (IsInitialized())
        {
            product = m_StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                //Debug.Log(string.Format("Product: '{0}'", product.definition.id));
            }
            else
            {
                Debug.LogError($"BuyProductID:{productId} FAIL. Not purchasing product, not found or not available for purchase. Check if ProductData with corresponding ID is in Resources/ProductData");
            }
        }
        return product;
    }

    public string GetPriceString(string productId)
    {
        Product product = GetProduct(productId);
        if (product != null)
        {
            System.Globalization.CultureInfo culture = GetCultureInfoFromISOCurrencyCode(product.metadata.isoCurrencyCode);
            if (culture != null)
            {
                return product.metadata.localizedPrice.ToString("C", culture);
            }
            else
            {
                // Fallback to just using localizedPrice decimal
                return product.metadata.localizedPriceString;
            }
        }
        return null;
    }

    public static System.Globalization.CultureInfo GetCultureInfoFromISOCurrencyCode(string code)
    {
        foreach (System.Globalization.CultureInfo ci in System.Globalization.CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
        {
            System.Globalization.RegionInfo ri = new System.Globalization.RegionInfo(ci.LCID);
            if (ri.ISOCurrencySymbol == code)
                return ci;
        }
        return null;
    }

    // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
    // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
    public void RestorePurchases()
    {
        // If Purchasing has not yet been set up ...
        if (!IsInitialized())
        {
            // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
            Debug.Log("RestorePurchases FAIL. Not initialized.");
            return;
        }

        // If we are running on an Apple device ... 
        if (Application.platform == RuntimePlatform.IPhonePlayer ||
            Application.platform == RuntimePlatform.OSXPlayer)
        {
            // ... begin restoring purchases
            Debug.Log("RestorePurchases started ...");

            // Fetch the Apple store-specific subsystem.
            var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
            // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
            // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
            apple.RestoreTransactions((result) =>
            {
                // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                // no purchases are available to be restored.
                Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
            });
        }
        // Otherwise ...
        else
        {
            // We are not running on an Apple device. No work is necessary to restore purchases.
            Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
        }
    }


    //  
    // --- IStoreListener
    //

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        // Purchasing has succeeded initializing. Collect our Purchasing references.
        Debug.Log("OnInitialized: PASS");

        // Overall Purchasing system, configured with products for this application.
        m_StoreController = controller;
        // Store specific subsystem, for accessing device-specific store features.
        m_StoreExtensionProvider = extensions;

        IAPProcessor.Init();
    }


    public void OnInitializeFailed(InitializationFailureReason error)
    {
        // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
        Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        bool isValidPurchase = IAPProcessor.OnPurchase(args);
        //if isValidPurchase was false, you should display an error message
        if (onPurchaseComplete != null)
        {
            onPurchaseComplete.Invoke(isValidPurchase, PurchaseProcessingResult.Complete, args.purchasedProduct.definition.id);
            onPurchaseComplete = null;
        }
        /*// A consumable product has been purchased by this user.
        if (CompareProductId(productIDDiamond1, args))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
        }
        // Or ... a non-consumable product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
        }
        // Or ... a subscription product has been purchased by this user.
        else if (String.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, StringComparison.Ordinal))
        {
            Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
            // TODO: The subscription item has been successfully purchased, grant this to the player.
        }
        // Or ... an unknown product has been purchased by this user. Fill in additional products here....
        else
        {
            Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
        }*/

        // Return a flag indicating whether this product has completely been received, or if the application needs 
        // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
        // saving purchased products to the cloud, and when that save is delayed. 
        return PurchaseProcessingResult.Complete;
    }

    public static bool CompareProductId(string productId, PurchaseEventArgs args)
    {
        return String.Equals(args.purchasedProduct.definition.id, productId, StringComparison.Ordinal);
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
        // this reason with the user to guide their troubleshooting actions.
        Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        FirebaseManager.LogCrashlytics(failureReason.ToString());
        FirebaseManager.LogException(new Exception("IAP Purchase Failed"));
    }

    public static bool CheckReceipt(string productId)
    {
        return CheckReceipt(instance.GetProduct(productId));
    }

    static bool CheckReceipt(Product purchasedProduct)
    {
        if (!instance.IsInitialized()) return false;
        bool validPurchase = true; // Presume valid for platforms with no R.V.

        // Unity IAP's validation logic is only included on these platforms.
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
        // Prepare the validator with the secrets we prepared in the Editor
        // obfuscation window.
        var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
            AppleTangle.Data(), Application.identifier);

        try
        {
            // On Google Play, result has a single product ID.
            // On Apple stores, receipts contain multiple products.
            var result = validator.Validate(purchasedProduct.receipt);
            // For informational purposes, we list the receipt(s)
            Debug.Log("Receipt is valid. Contents:");
            foreach (IPurchaseReceipt productReceipt in result)
            {
                Debug.Log(productReceipt.productID);
                Debug.Log(productReceipt.purchaseDate);
                Debug.Log(productReceipt.transactionID);
            }
        }
        catch (IAPSecurityException)
        {
            Debug.Log("Invalid receipt, not unlocking content");
            validPurchase = false;
        }
#endif

        return validPurchase;
    }

    static void LogError(string msg)
    {
        Debug.LogError(msg);
        FirebaseManager.LogEvent("IAP_Error", "message", msg);
    }
}