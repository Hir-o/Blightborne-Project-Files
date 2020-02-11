using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class HiddenLadderPlatform : MonoBehaviour {

	private float controlVertical;
	private Collider2D thisCollider;

	private void Start() 
	{
		thisCollider = GetComponent<Collider2D>();
	}

	private void Update() 
	{
		controlVertical = CrossPlatformInputManager.GetAxis("Vertical");
		
		if (controlVertical < 0f)
			thisCollider.enabled = false;
		else if (controlVertical > 0f)
			thisCollider.enabled = true;
	}

}
