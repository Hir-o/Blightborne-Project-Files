using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlatformsMessage : MonoBehaviour {

	void Start () 
	{
		if (PlayerStats.CheckpointPriority > 0)
			this.gameObject.SetActive(false);	
	}
	
	
}
