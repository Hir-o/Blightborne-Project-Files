using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FourMovingPlatforms : MonoBehaviour
{

	[SerializeField] private Transform destination;
	
	private void Start()
	{
		if (PlayerStats.CheckpointPriority == 3) transform.position = destination.position;
		else transform.position = new Vector3(PlayerStats.CheckpointLocation.x - 4.5f, 0f ,0f);
	}

}
