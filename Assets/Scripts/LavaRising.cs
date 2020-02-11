using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hellmade.Sound;

public class LavaRising : MonoBehaviour {

	public float lavaRisingSpeed = 2f;
	[SerializeField] private float offsetY = -24f;
	[SerializeField] private float offsetYLastCheckpoint = -50f;

	[SerializeField] private float distance;

	public static bool StopRising = false;

	private void Awake() 
	{
		if (PlayerStats.CheckpointPriority == 2)
			transform.position = new Vector3(transform.position.x, offsetYLastCheckpoint, transform.position.z);
		else
			transform.position = new Vector3(transform.position.x, offsetY, transform.position.z);
	}

	private void Start()
	{
		transform.parent = null;
		transform.position = new Vector3(0f, transform.position.y, transform.position.z);
	}

	private void Update() 
	{
		if (StopRising) { return; }

		transform.Translate(Vector3.up * lavaRisingSpeed * Time.deltaTime, Space.World);	

		distance = Vector2.Distance(this.transform.position, GameManager.player.transform.position);

		if (distance <= 25f)
		{
			//Icrease Music Volume
			if (EazySoundManager.GlobalMusicVolume != 0f)
				AudioController.Instance.IncreaseEscapeThemeVolume(.5f);
		}
		else
		{
			//Decrease Music Volume
			if (EazySoundManager.GlobalMusicVolume != 0f)
				AudioController.Instance.DecreaseEscapeThemeVolume(.3f);
		}
	}

}
