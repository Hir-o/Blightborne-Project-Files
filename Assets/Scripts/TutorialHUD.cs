using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialHUD : MonoBehaviour {

	[SerializeField] private GameObject abilityPanel;
	[SerializeField] private GameObject abilityKeysPanel;

    public void EnableAbilityPanel()
    {	
        abilityPanel.SetActive(true);
		abilityKeysPanel.SetActive(true);
    }

}
