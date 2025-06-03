//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Purchasing;
//using System;
//using Unity.Services.Core;
//using UnityEngine.Purchasing.Extension;



//[Serializable]



//public class ConsumableItem
//{
//    public string Name;
//    public string Id;
//    public string Description;
//    public float Price;
//}

//public class ShopScript : MonoBehaviour, IDetailedStoreListener
//{
//    private IStoreController storeController;

//    [Header("Consumable Items")]
//    public ConsumableItem item100;
//    public ConsumableItem item300;
//    public ConsumableItem item1000;
//    public ConsumableItem item10000;

//    public RewerdCollectionGold collectgold;

//    //public Text coinText;
//    public Text messageText;

//    private async void Start()
//    {
//        try
//        {
//            await UnityServices.InitializeAsync();
//            ShowMessage("Unity Gaming Services initialized successfully.");
//        }
//        catch (Exception e)
//        {
//            ShowMessage($"Failed to initialize Unity Gaming Services: {e.Message}");
//            return;
//        }

//        int coins = PlayerPrefs.GetInt("totalCoins", 0);

//        InitializePurchasing();
//    }

//    #region Purchasing Setup

//    void InitializePurchasing()
//    {
//        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
//        builder.AddProduct(item100.Id, ProductType.Consumable);
//        builder.AddProduct(item300.Id, ProductType.Consumable);
//        builder.AddProduct(item1000.Id, ProductType.Consumable);
//        builder.AddProduct(item10000.Id, ProductType.Consumable);
//        UnityPurchasing.Initialize(this, builder);
//    }

//    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//    {
//        ShowMessage("IAP initialized successfully.");
//        storeController = controller;
//    }

//    public void OnInitializeFailed(InitializationFailureReason error)
//    {
//        ShowMessage($"IAP Initialization failed: {error}");
//    }

//    public void OnInitializeFailed(InitializationFailureReason error, string message)
//    {
//        ShowMessage($"IAP Initialization failed: {error}, {message}");
//    }

//    #endregion

//    #region Purchase Handling

//    public void BuyItem100()
//    {
//        if (storeController != null)
//        {
//            ShowMessage($"Attempting to purchase: {item100.Id}");
//            storeController.InitiatePurchase(item100.Id);
//            //ADD GOLD EFFECT
            
            
            
//        }
//        else
//        {
//            ShowMessage("Purchase failed: Store controller is not initialized.");
//        }
//    }

//    public void BuyItem300()
//    {
//        if (storeController != null)
//        {
//            ShowMessage($"Attempting to purchase: {item300.Id}");
//            storeController.InitiatePurchase(item300.Id);
            
//        }
//        else
//        {
//            ShowMessage("Purchase failed: Store controller is not initialized.");
//        }
//    }

//    public void BuyItem1000()
//    {
//        if (storeController != null)
//        {
//            ShowMessage($"Attempting to purchase: {item1000.Id}");
//            storeController.InitiatePurchase(item1000.Id);
            
//        }
//        else
//        {
//            ShowMessage("Purchase failed: Store controller is not initialized.");
//        }
//    }

//    public void BuyItem10000()
//    {
//        if (storeController != null)
//        {
//            ShowMessage($"Attempting to purchase: {item10000.Id}");
//            storeController.InitiatePurchase(item10000.Id);
            
//        }
//        else
//        {
//            ShowMessage("Purchase failed: Store controller is not initialized.");
//        }
//    }

//    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
//    {
//        if (args.purchasedProduct.definition.id == item100.Id)
//        {
//            GoldToSteelConverter.Instance.AddGold100();
//            collectgold.OnButtonClickedGold();
//            ShowMessage("Consumable item 1 purchased.");
//        }
//        else if (args.purchasedProduct.definition.id == item300.Id)
//        {
//            GoldToSteelConverter.Instance.AddGold300();
//            collectgold.OnButtonClickedGold();
//            ShowMessage("Consumable item 2 purchased.");
//        }
//        else if (args.purchasedProduct.definition.id == item1000.Id)
//        {
//            GoldToSteelConverter.Instance.AddGold1000();
//            collectgold.OnButtonClickedGold();
//            ShowMessage("Consumable item 3 purchased.");
//        }
//        else if (args.purchasedProduct.definition.id == item10000.Id)
//        {
//            GoldToSteelConverter.Instance.AddGold10000();
//            collectgold.OnButtonClickedGold();
//            ShowMessage("Consumable item 4 purchased.");
//        }

//        return PurchaseProcessingResult.Complete;
//    }

//    public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
//    {
//        ShowMessage($"Purchase failed: {failureDescription.reason} - {failureDescription.message}");
//    }

//    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
//    {
//        ShowMessage($"Purchase failed: {failureReason}");
//    }

//    #endregion



//    #region Adgold Functions


//    #endregion

//    #region UI Handling

//    private void ShowMessage(string message)
//    {
//        messageText.text = message;
//        Debug.Log(message);
//    }

//    #endregion
//}