using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopPanel : MonoBehaviour
{
    [Header("Trader Dialogue Box")]
    [SerializeField]
    private TraderStaticDialogue traderDialogue;

    [Header("Buttons")]
    [SerializeField]
    private Button pendantButton;

    [SerializeField] private Button ringButton;
    [SerializeField] private Button pyramidButton;

    [Header("Items Cost")]
    [SerializeField]
    private int pendantCost;

    [SerializeField] private int ringCost;
    [SerializeField] private int pyramidCost;

    private void Start () { UpdateButtons(); }

    private void UpdateButtons ()
    {
        //Disable buttons if items are purchased
        if (PlayerStats.IsPendantPurchased)
        {
            pendantButton.GetComponent<Button>().interactable = false;
            pendantButton.GetComponent<Animator>().SetTrigger("Disabled");
        }

        if (PlayerStats.IsRingPurchased)
        {
            ringButton.GetComponent<Button>().interactable = false;
            ringButton.GetComponent<Animator>().SetTrigger("Disabled");
        }

//		if (PlayerStats.IsPyramidPurchased)
//		{
//			pyramidButton.GetComponent<Button>().interactable = false;	
//			pyramidButton.GetComponent<Animator>().SetTrigger("Disabled");
//		}

        if (PlayerStats.IsEnergyPyramidPurchased)
        {
            pyramidButton.GetComponent<Button>().interactable = false;
            pyramidButton.GetComponent<Animator>().SetTrigger("Disabled");
        }

        //end
    }

    public void BuyPendant ()
    {
        if (PlayerStats.Coins >= pendantCost && PlayerStats.IsPendantPurchased == false)
        {
            PlayerStats.Coins              -= pendantCost;
            PlayerStats.IsPendantPurchased =  true;
            PlayerStats.IsPendantReady     =  true;
            EssentialObjects.UpdateCoinsStatic();
            GameManager.hud.UpdateItemsIcons();
            AudioController.Instance.PurchaseSFX();

//            PokiUnitySDK.Instance.happyTime(0.6f);
        }
        else
        {
            traderDialogue.NotEnoughCashDialogue();
            AudioController.Instance.ButtonClickSFX();
        }

        UpdateButtons();
    }

    public void BuyRing ()
    {
        if (PlayerStats.Coins >= ringCost && PlayerStats.IsRingPurchased == false)
        {
            PlayerStats.Coins           -= ringCost;
            PlayerStats.IsRingPurchased =  true;
            EssentialObjects.UpdateCoinsStatic();
            GameManager.hud.UpdateItemsIcons();
            AudioController.Instance.PurchaseSFX();

//            PokiUnitySDK.Instance.happyTime(0.6f);
        }
        else
        {
            traderDialogue.NotEnoughCashDialogue();
            AudioController.Instance.ButtonClickSFX();
        }

        UpdateButtons();
    }

    public void BuyPyramid ()
    {
        if (PlayerStats.Coins >= pyramidCost && PlayerStats.IsPyramidPurchased == false)
        {
            PlayerStats.Coins              -= pyramidCost;
            PlayerStats.IsPyramidPurchased =  true;
            EssentialObjects.UpdateCoinsStatic();
            GameManager.hud.UpdateItemsIcons();
            AudioController.Instance.PurchaseSFX();

//            PokiUnitySDK.Instance.happyTime(0.6f);
        }
        else
        {
            traderDialogue.NotEnoughCashDialogue();
            AudioController.Instance.ButtonClickSFX();
        }

        UpdateButtons();
    }

    public void BuyEnergyPyramid ()
    {
        if (PlayerStats.Coins >= pyramidCost && PlayerStats.IsEnergyPyramidPurchased == false)
        {
            PlayerStats.Coins                    -= pyramidCost;
            PlayerStats.IsEnergyPyramidPurchased =  true;
            EssentialObjects.UpdateCoinsStatic();
            GameManager.hud.UpdateItemsIcons();
            AudioController.Instance.PurchaseSFX();
            
//            PokiUnitySDK.Instance.happyTime(0.6f);
        }
        else
        {
            traderDialogue.NotEnoughCashDialogue();
            AudioController.Instance.ButtonClickSFX();
        }

        UpdateButtons();
    }
}