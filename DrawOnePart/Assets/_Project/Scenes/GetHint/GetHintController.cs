using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SS.View;
using TMPro;
using UnityEngine.Purchasing;

public class GetHintController : Controller
{
    public const string GETHINT_SCENE_NAME = "GetHint";

    public override string SceneName()
    {
        return GETHINT_SCENE_NAME;
    }

    [SerializeField] TMP_Text buyHintPriceText;
    private void Start()
    {
        buyHintPriceText.text = InAppPurchaseHelper.instance.GetPriceString(IAPProcessor.get_10_hints);
    }

    public void RewardGetHint()
    {
        FirebaseManager.LogEvent("GetHint_RewardGetHint_BtnClick");
        AdsManager.Reward((bool success) =>
        {
            if (success)
            {
                User.AddHint(1);
                Manager.Add(ReceiveItemController.RECEIVEITEM_SCENE_NAME, new PopupData(PopupType.OK, "+1", "You received a hint!"));
            }
        }, AdPlacementType.Reward_GetMoreHint);
    }

    public void BuyHint()
    {
        FirebaseManager.LogEvent("GetHint_IAPBuyHint_BtnClick");
        InAppPurchaseHelper.instance.BuyProduct(IAPProcessor.get_10_hints, (bool success, PurchaseProcessingResult result, string productID) =>
        {
            if (success)
            {
                FirebaseManager.LogEvent("GetHint_IAPBuyHint_Success");
                Manager.Add(ReceiveItemController.RECEIVEITEM_SCENE_NAME, new PopupData(PopupType.OK, "+10", "Purchase successful! You got 10 hints."));
            }
            else
            {
                FirebaseManager.LogEvent("GetHint_IAPBuyHint_Failed");
                Manager.Add(PopupController.POPUP_SCENE_NAME, new PopupData(PopupType.OK, $"There was a problem processing {productID}, please contact the developer."));
            }
        });
    }
}