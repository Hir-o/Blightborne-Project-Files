using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCleaner : MonoBehaviour {

	[SerializeField] private float timer = 10f;

	void Start () 
	{
		Destroy(gameObject, timer);
	}
	
}
