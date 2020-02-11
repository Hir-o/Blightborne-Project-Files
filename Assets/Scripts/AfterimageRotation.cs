using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterimageRotation : MonoBehaviour {

	private float sign;
	private float xSize;

	private void Start() {
		xSize = transform.localScale.x;
	}

	public void Rotate(Transform playerTransform) // called from animation event in player "RotateAfterImage"
	{
		sign = Mathf.Sign(playerTransform.localScale.x);
		transform.localScale = new Vector3(sign * xSize, transform.localScale.y, transform.localScale.z);
	}
}
