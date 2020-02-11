using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Image portrait;
    public GameObject dialogueBox;
    //public GameObject nameBox;

    public string [] dialogLines; 

    public int currentLine;
    private bool justStarted;

    private enum DialogueState {Typing, None}
    private DialogueState dialogueState = DialogueState.None;

    public static DialogManager instance;

    private Player player;

    private int frame;
    private float pitchLevel;

    private void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;	
	}

	 void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

	private void Awake() {
		dialogueBox = GameObject.Find("Dialogue Box").gameObject;
		dialogueBox.SetActive(false);
	}

    void Start()
    {
        instance = this;
        player = GameObject.Find("Player").GetComponent<Player>();
        //dialogueText.text = dialogLines[currentLine]; 
    }

    void Update()
    {
        if (dialogueBox.activeInHierarchy)
        {
            if(InputControl.GetButtonUp("Next"))
            {
                if(!justStarted)
                {
                    if (dialogueState == DialogueState.None)
                        currentLine++;

                    if(currentLine >= dialogLines.Length)
                    {
                        dialogueBox.SetActive(false);
                        player.isDeactivated = false;
                    }
                    else
                    {
                        StopAllCoroutines();
                        StartCoroutine(TypeSentence(dialogLines[currentLine]));
                    } 
                }
                else
                {
                    justStarted = false;
                }  
            }

            if (InputControl.GetButtonDown("Close"))
            {
                currentLine = 0;
                justStarted = false;
                dialogueText.text = "";
                dialogueState = DialogueState.None;
                player.isDeactivated = false;
                StopAllCoroutines();

                dialogueBox.SetActive(false);

                Invoke(nameof(ResetDialogueState), .2f);
            }
        }
    }

    private void ResetDialogueState()
    {
//        GameManager.IsDialogueActive = false;
    }

    public void ShowDialog(string [] newLines , bool isPerson, float newPitchLevel)
    {
        pitchLevel = newPitchLevel;
        dialogLines = newLines;
        currentLine = 0;

        dialogueBox.SetActive(true);

        justStarted = true;
        player.isDeactivated = true;

        StopAllCoroutines();
		StartCoroutine(TypeSentence(dialogLines[currentLine]));
    }

    private IEnumerator TypeSentence(string sentence)
	{
        if (dialogueState == DialogueState.Typing)
        {
            dialogueState = DialogueState.None;
            dialogueText.text = sentence;
//            GameManager.IsDialogueActive = false;

            yield break;
        }

		dialogueText.text = "";

        dialogueState = DialogueState.Typing;

		foreach(char letter in sentence.ToCharArray())
		{
			dialogueText.text += letter;
            frame++;

            if (frame % 3 == 0)
			    AudioController.Instance.CharacterTypoSFX(pitchLevel);

			yield return new WaitForSeconds(.02f);
		}

        dialogueState = DialogueState.None;
	}

    public void SetPortrait(Sprite newPortrait)
    {
        portrait.sprite = newPortrait;
    }

    public void SetName(string newName)
    {
        nameText.text = newName;
    }
}
