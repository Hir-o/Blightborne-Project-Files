using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeParent : MonoBehaviour {

	[SerializeField] private float timer = 1f;

	private void Start() 
	{
		Invoke(nameof(InitializeOutlines), timer);
	}

	private void InitializeOutlines()
	{
		transform.parent = null;
	}
}
