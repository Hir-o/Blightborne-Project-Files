using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RestoreFocus : MonoBehaviour {

	[SerializeField] Button[] buttons;

	[SerializeField] private bool isAbilityPanel;

	private void OnEnable() 
	{
		if (isAbilityPanel)
			EventSystem.current.SetSelectedGameObject(null);
	}
	
	public void SelectButton()
	{
		EventSystem.current.SetSelectedGameObject(null);
	}

	void Update () 
	{         
		if (EventSystem.current.currentSelectedGameObject == null)
			foreach (Button tempButton in buttons)
				if (tempButton.interactable && tempButton.transform.parent.gameObject.activeSelf)
					EventSystem.current.SetSelectedGameObject(tempButton.gameObject);
	}
}
