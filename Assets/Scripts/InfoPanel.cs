using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class InfoPanel : MonoBehaviour {

	[SerializeField] private GameObject infoPanel;
	public static bool OpenInfoPanel = true;

	private void Start() 
	{
		if (PlayerStats.IsFirstPlay && OpenInfoPanel)
		{
			infoPanel.SetActive(true);
			OpenInfoPanel = false;
			GameManager.IsInfoPanelOpen = true;
			GameManager.player.isDeactivated = true;
		}
	}

	private void Update() 
	{
		if (infoPanel.activeSelf)
		{
			if (InputControl.GetButtonDown("Close"))
            {
				infoPanel.SetActive(false);
				GameManager.player.isDeactivated = false;
                Invoke(nameof(ResetInfoPanelState), .2f);
            }
		}
	}

	private void ResetInfoPanelState()
	{
		GameManager.IsInfoPanelOpen = false;
	}
}
