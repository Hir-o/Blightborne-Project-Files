using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaCheckpoint : MonoBehaviour {

	[SerializeField] private LavaRising lava;
	[SerializeField] private float lavaRisingAdder = 1f;
	private bool lavaSpedUp = false;

	private void Start() 
	{
		lava = FindObjectOfType<LavaRising>();
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (lavaSpedUp == true) 
		{ 
			this.enabled = false;
			return;	
		}
		
		if (other.CompareTag(Tag.PlayerTag))
		{
			if (other.GetType() == typeof(CapsuleCollider2D))
			{
				lava.lavaRisingSpeed += lavaRisingAdder;
				lavaSpedUp = true;
			}
		}	
	}

}
