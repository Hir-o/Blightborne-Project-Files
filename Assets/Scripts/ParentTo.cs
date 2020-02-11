using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentTo : MonoBehaviour {

	private void Start() 
	{
		transform.SetParent(GameManager.cameraTransition.transform);	
	}
}
