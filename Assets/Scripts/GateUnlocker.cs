using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateUnlocker : MonoBehaviour {

	[SerializeField] private Animator[] animators;

	private void Start() 
	{
		foreach (Animator anim in animators)
		{
			anim.SetBool("isLocked", false);
		}	
	}
}
