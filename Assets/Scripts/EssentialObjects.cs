using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EssentialObjects : MonoBehaviour {

	public static TextMeshProUGUI COINS;

	[SerializeField] private TextMeshProUGUI textCoins;

	private void Awake() 
	{
		COINS = textCoins;
	}

	public static void UpdateCoinsStatic()
	{
		if (PlayerStats.DisableBagSystem) 
			GameManager.hud.coinsText.text = PlayerStats.Coins.ToString();
		else
			GameManager.hud.coinsText.text = PlayerStats.Coins + "/" + PlayerStats.MaxCoinCapacity;
	}

	public void UpdateCoins()
	{
		if (PlayerStats.DisableBagSystem) 
			GameManager.hud.coinsText.text = COINS.text;
		else
			GameManager.hud.coinsText.text = COINS.text + "/" + PlayerStats.MaxCoinCapacity;
	}

}
