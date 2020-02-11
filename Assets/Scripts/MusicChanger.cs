using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChanger : MonoBehaviour {

	private Collider2D collider;
	private bool isMusicChanged;

	private void Start() 
	{
		collider = GetComponent<Collider2D>();
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.gameObject.CompareTag(Tag.PlayerTag))	
		{
			if (other.GetComponent<Collider2D>().GetType() == typeof(CapsuleCollider2D))
			{
				AudioController.Instance.PlayMiniBossTheme();
				isMusicChanged = true;
				
				if (isMusicChanged)
					collider.enabled = false;
			}
		}
	}

	private void OnTriggerStay2D (Collider2D other)
	{
		if (other.gameObject.CompareTag(Tag.PlayerTag))
		{
			if (other.GetComponent<Collider2D>().GetType() == typeof (CapsuleCollider2D))
			{
				AudioController.Instance.PlayMiniBossTheme();
				isMusicChanged = true;
				
				if (isMusicChanged)
					collider.enabled = false;
			}
		}
	}
}
