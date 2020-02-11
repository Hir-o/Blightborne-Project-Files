using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CinemachineVirtualCameraStore : MonoBehaviour
{
	public GameObject runVirtualCammera;

	[Header("Shake Values")]
	[SerializeField]
	private float duration = .2f;
	[SerializeField] private float strength = .5f;
	[SerializeField] private int vibratio = 14;
	[SerializeField] private float randomness = 90f;
	[SerializeField] private bool snapping;
	[SerializeField] private bool fadeOut = true;

	public void ShakeCamera ()
	{
		runVirtualCammera.transform
			.DOShakePosition(duration, strength, vibratio, randomness, snapping, fadeOut);
	}
}
