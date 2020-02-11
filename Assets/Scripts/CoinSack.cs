using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSack : MonoBehaviour {

	[Header("Capacity Levels")]
	[SerializeField] private int startingCapacity = 50;
	[SerializeField] private int level1Capacity = 100;
	[SerializeField] private int level2Capacity = 200;
	[SerializeField] private int level3Capacity = 400;

	[Header("Coins")]
	[SerializeField] private Coin[] coins;
	[SerializeField] private Vector3 offset;
	[SerializeField] private float throwTimer = .1f;

	[Header("Point Effector GameObject")]
	[SerializeField] private GameObject pointEffector;

	private int coinDifference;
	private IEnumerator coroutine;
	private Coin coin;
	private Player player;
	
	private void Awake() 
	{
		player = GetComponent<Player>();	
	}

	private void Start() 
	{
		if (PlayerStats.DisableBagSystem) { return; }

		UpdateMaxCoinCapacity();
	}

	public void UpdateMaxCoinCapacity()
	{
		if (PlayerStats.DisableBagSystem) { return; }

		switch (PlayerStats.CoinSackIndex)
		{
			case 0: PlayerStats.MaxCoinCapacity = startingCapacity;
				break;
			case 1: PlayerStats.MaxCoinCapacity = level1Capacity;
				break;
			case 2: PlayerStats.MaxCoinCapacity = level2Capacity;
				break;
			case 3: PlayerStats.MaxCoinCapacity = level3Capacity;
				break;
			default:
				break;
		}

		EssentialObjects.UpdateCoinsStatic();
	}

	public void CheckForCoinCapacity()
	{
		if (PlayerStats.DisableBagSystem) { return; }
		
		if (PlayerStats.Coins > PlayerStats.MaxCoinCapacity)
		{
			pointEffector.SetActive(false);
			
			coinDifference = PlayerStats.Coins - PlayerStats.MaxCoinCapacity;

			if (coroutine != null)
			{
				StopCoroutine(coroutine);
			}

			coroutine = ThrowCoin();
			StartCoroutine(coroutine);
		}
		else
		{
			pointEffector.SetActive(true);
		}
	}

	private IEnumerator ThrowCoin()
	{
		for (int i = 0; i < coinDifference; i++)
		{
			coin = Instantiate(coins[Random.Range(0, coins.Length)], transform.position + offset, Quaternion.identity);
			coin.playerScaleZ = player.transform.localScale.x;
			PlayerStats.Coins--;
			EssentialObjects.UpdateCoinsStatic();

			yield return new WaitForEndOfFrame();
		}
	}
}
