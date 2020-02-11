using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStartingPosition : MonoBehaviour {

	private void Start () 
	{
		if (SceneManager.GetActiveScene().buildIndex >= 3)
			transform.position = PlayerStats.CheckpointLocation;
	}

}
