using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagsUnlocker : MonoBehaviour {

	[Header("Trader Dialogue Box")]
	[SerializeField] private TraderStaticDialogue traderDialogue;

	[Header("Buttons")]
	[SerializeField] private Button pouchButton;	
	[SerializeField] private Button medBagButton;	
	[SerializeField] private Button bigBagButton;	

	[Header("Bag Cost")]
	[SerializeField] private int pouchCost;
	[SerializeField] private int medBagCost;
	[SerializeField] private int bigBagCost;

	private CoinSack coinSack;

	private void Start() 
	{
		pouchButton.GetComponent<Animator>().SetBool("isClickable", true);
		coinSack = FindObjectOfType<CoinSack>();
		
		UpdateButtons();
	}

	private void UpdateButtons()
	{
		if (PlayerStats.CoinSackIndex == 1)
		{
			pouchButton.GetComponent<Button>().interactable = false;	
			pouchButton.GetComponent<Animator>().SetTrigger("Disabled");

			medBagButton.GetComponent<Animator>().SetBool("isClickable", true);
		}
		if (PlayerStats.CoinSackIndex == 2)
		{
			medBagButton.GetComponent<Button>().interactable = false;	
			medBagButton.GetComponent<Animator>().SetTrigger("Disabled");	

			bigBagButton.GetComponent<Animator>().SetBool("isClickable", true);
		}
		if (PlayerStats.CoinSackIndex == 3)
		{
			bigBagButton.GetComponent<Button>().interactable = false;	
			bigBagButton.GetComponent<Animator>().SetTrigger("Disabled");
		}
	}

	public void BuyPouch()
	{
		if (PlayerStats.Coins >= pouchCost && PlayerStats.CoinSackIndex == 0)
		{
			PlayerStats.CoinSackIndex++;
		
			PlayerStats.Coins -= pouchCost;
			EssentialObjects.UpdateCoinsStatic();
			coinSack.UpdateMaxCoinCapacity();
			coinSack.CheckForCoinCapacity();
		}
		else
		{
			traderDialogue.NotEnoughCashDialogue();
		}

		UpdateButtons();
	}
	
	public void BuyMediumBag()
	{
		if (PlayerStats.Coins >= medBagCost && PlayerStats.CoinSackIndex == 1)
		{
			PlayerStats.CoinSackIndex++;

			PlayerStats.Coins -= medBagCost;
			EssentialObjects.UpdateCoinsStatic();
			coinSack.UpdateMaxCoinCapacity();
			coinSack.CheckForCoinCapacity();
		}
		else
		{
			traderDialogue.NotEnoughCashDialogue();
		}

		UpdateButtons();
	}

	public void BuyBigBag()
	{
		if (PlayerStats.Coins >= bigBagCost && PlayerStats.CoinSackIndex == 2)
		{
			PlayerStats.CoinSackIndex++;

			PlayerStats.Coins -= bigBagCost;
			EssentialObjects.UpdateCoinsStatic();
			coinSack.UpdateMaxCoinCapacity();
			coinSack.CheckForCoinCapacity();
		}
		else
		{
			traderDialogue.NotEnoughCashDialogue();
		}

		UpdateButtons();
	}

}
