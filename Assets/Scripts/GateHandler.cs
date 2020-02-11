using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateHandler : MonoBehaviour {

	private Animator animator;

	private void Awake() 
	{
		animator = GetComponent<Animator>();	
	}

	private void Start() 
	{
		animator.SetBool("isLocked", false);	
	}
	
}
