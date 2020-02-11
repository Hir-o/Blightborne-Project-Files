using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveCount : MonoBehaviour {

	private void Start() 
	{
		Physics2D.IgnoreLayerCollision(12, 13, true);	
	}
}
