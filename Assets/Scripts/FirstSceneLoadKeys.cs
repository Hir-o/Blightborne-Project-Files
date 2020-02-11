using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstSceneLoadKeys : MonoBehaviour {

	private void Start() 
	{
		if (PlayerStats.IsFirstPlay == false)
			Destroy(gameObject);
	}
}
