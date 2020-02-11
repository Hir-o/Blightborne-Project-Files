using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheckpoint : MonoBehaviour {

	public void ResetPlayerToCheckpoint() // called by animation event
	{
		GameManager.Instance.ReturnToCheckpoint();
	}
}
