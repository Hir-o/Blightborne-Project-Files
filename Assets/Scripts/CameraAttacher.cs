using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAttacher : MonoBehaviour {

	private Canvas canvas;
	[SerializeField] private bool isEndGameCanvas;
	[SerializeField] private bool isVillageCanvas;

	private void Start() 
	{
		canvas = GetComponent<Canvas>();

		canvas.worldCamera = GameManager.cameraTransition.GetComponent<Camera>();

		if (isEndGameCanvas || isVillageCanvas)
			canvas.sortingLayerName = "UI";
	}
}
