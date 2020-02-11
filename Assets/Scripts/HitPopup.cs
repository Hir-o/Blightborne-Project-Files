using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HitPopup : MonoBehaviour {

	private TextMeshPro textMesh;

	[SerializeField] private Color[] randomColors;
	[SerializeField] private string[] hitTexts;

	[SerializeField] private float destroyTimer = 4f;
	[SerializeField] private float randomRotateValue = 15f;

	private int index;
	private int randomIndexColor;

	private void Awake() 
	{
		textMesh = GetComponent<TextMeshPro>();
		transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-randomRotateValue, randomRotateValue));
		Setup();
	}

	public void Setup()
	{
		index = Random.Range(0, hitTexts.Length);
		randomIndexColor = Random.Range(0, randomColors.Length);

		textMesh.SetText(hitTexts[index]);
		textMesh.color = randomColors[randomIndexColor];

		Destroy(transform.parent.gameObject, destroyTimer);
	}
}
