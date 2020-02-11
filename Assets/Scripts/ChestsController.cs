using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestsController : MonoBehaviour {

	public static int[] chests = new int[100];

	public static void LoadChestsState() 
	{
		for (int i = 0; i < chests.Length; i++)
			chests[i] = PlayerPrefs.GetInt("Chest" + i);
	}

	public static void SaveChestsState()
	{
		for (int i = 0; i < chests.Length; i++)
			if (chests[i] == 1)
				PlayerPrefs.SetInt("Chest" + i, 1);
	}

	public static void ResetChestsState()
	{
		for (int i = 0; i < chests.Length; i++)
		{
			//if (chests[i] == 1)
				chests[i] = 0;
			PlayerPrefs.DeleteKey("Chest" + i);
		}
	}

}
