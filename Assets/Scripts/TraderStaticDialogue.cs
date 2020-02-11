using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TraderStaticDialogue : MonoBehaviour {

	[Header("Generic Code")]
	[SerializeField] private TextMeshProUGUI dialogue;
	[TextArea(1,10)][SerializeField] private string startingText;
	[TextArea(1,10)][SerializeField] private string notEnoughCashText;

	[Header("Ability Panel Code")]
	[TextArea(1,10)][SerializeField] private string notEnoughSkillPointsText;

	private IEnumerator coroutine;

	private int frame;

	[SerializeField] private float pitchLevel;

	private void OnEnable()
	{
		coroutine = Dialogue(startingText);
		StartCoroutine(coroutine);
	}
	
	private void OnDisable() 
	{
		StopCoroutine(coroutine);
	}

	private IEnumerator Dialogue(string sentence)
	{
		dialogue.text = "";

		foreach(char letter in sentence.ToCharArray())
		{
			dialogue.text += letter;
			frame++;

			if (frame % 3 == 0)
			    AudioController.Instance.CharacterTypoSFX(pitchLevel);

			yield return new WaitForSeconds(.02f);
		}
	}

	public void StartingDialogue()
	{
		StopCoroutine(coroutine);

		coroutine = Dialogue(startingText);
		StartCoroutine(coroutine);
	}

	public void NotEnoughCashDialogue()
	{
		StopCoroutine(coroutine);

		coroutine = Dialogue(notEnoughCashText);
		StartCoroutine(coroutine);
	}

	public void NotEnoughSkillPointsDialogue()
	{
		StopCoroutine(coroutine);
		
		coroutine = Dialogue(notEnoughSkillPointsText);
		StartCoroutine(coroutine);
	}
}
