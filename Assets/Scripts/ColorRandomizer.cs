using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour {

	[SerializeField] Color[] colors;

	private SpriteRenderer sprite;

	private void Start() 
	{
		sprite = GetComponent<SpriteRenderer>();
		sprite.color = colors[Random.Range(0, colors.Length)];
	}

}
