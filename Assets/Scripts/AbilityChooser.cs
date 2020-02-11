using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class AbilityChooser : MonoBehaviour 
{
	public static AbilityChooser Instance;
	[SerializeField] private Texture[] abilityTextures;
	[SerializeField] private RawImage currentSkillTexture;
	[SerializeField] private RawImage nextSkillTexture;
	[SerializeField] private RawImage previousSkillTexture;

	private int currentIndex;

	private void Awake() 
	{
		if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
	}
	
	private void Start() 
	{
		UpdateTextures();
	}
	
	private void Update() 
	{
		if (InputControl.GetButtonDown("Scroll Skill Left"))
		{
			if (PlayerStats.CurrentAbilitySelctedIndex == 0)
				PlayerStats.CurrentAbilitySelctedIndex = PlayerStats.MaxAbilityIndex;
			else			
				PlayerStats.CurrentAbilitySelctedIndex--;

			UpdateTextures();
		}	
		else if (InputControl.GetButtonDown("Scroll Skill Right"))
		{
			if (PlayerStats.CurrentAbilitySelctedIndex == PlayerStats.MaxAbilityIndex)
				PlayerStats.CurrentAbilitySelctedIndex = 0;
			else
				PlayerStats.CurrentAbilitySelctedIndex++;

			UpdateTextures();
		}
	}

	public void UpdateTextures()
	{
		currentIndex = PlayerStats.CurrentAbilitySelctedIndex;

		currentSkillTexture.texture = abilityTextures[currentIndex];

		if (PlayerStats.CurrentAbilitySelctedIndex == PlayerStats.MaxAbilityIndex)
			nextSkillTexture.texture = abilityTextures[0];
		else
			nextSkillTexture.texture = abilityTextures[(currentIndex + 1)];

		if (PlayerStats.CurrentAbilitySelctedIndex == 0)
			previousSkillTexture.texture = abilityTextures[PlayerStats.MaxAbilityIndex];
		else
			previousSkillTexture.texture = abilityTextures[(currentIndex - 1)];
	}
}
