using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineOffset : Singleton<CinemachineOffset> {

	[SerializeField] private CinemachineVirtualCamera vcam;
	private CinemachineFramingTransposer transposer;

	[SerializeField] private float offsetYValue = 0.15f;
	public float originalYVal, newYVal;

	[Header("Lerp time")] [SerializeField] private float lerpTimeValue = .5f;

	private void Awake() 
	{
		transposer = vcam.GetCinemachineComponent<CinemachineFramingTransposer>();
		originalYVal = transposer.m_ScreenY;
		newYVal = originalYVal;
	}

	private void Update()
	{
		if (vcam.m_Lens.OrthographicSize != GameManager.mainCamera.orthographicSize)
			vcam.m_Lens.OrthographicSize = GameManager.mainCamera.orthographicSize;
	}

	public void OffsetCamera()
	{
		transposer.m_ScreenY = Mathf.Lerp(transposer.m_ScreenY, offsetYValue, lerpTimeValue);
	}

	public void ResetCamera()
	{
		if (transposer.m_ScreenY == originalYVal) { return; }
		transposer.m_ScreenY = Mathf.Lerp(transposer.m_ScreenY, newYVal, lerpTimeValue);
	}
}
