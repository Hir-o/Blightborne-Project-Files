using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogActivator : MonoBehaviour
{
    [TextArea(1,10)]
    public string [] lines;

    private bool canActivate;
    [SerializeField] private bool isPerson = true;
    [SerializeField] private Sprite portraitImage;
    [SerializeField] private GameObject buttonSprite;
    [SerializeField] private string npcName;

    private Player player;
    private float buttonXSize;

    private enum DialogueState { enableDialogue, disableDialogue};
    private DialogueState dialogueState = DialogueState.enableDialogue;

    [SerializeField] private float pitchLevel;

    [SerializeField]
    private Animator _animator;

    private void Start()
    {
        player = GameManager.player;
        _animator = buttonSprite.GetComponent<Animator>();
    }

    private void Update()
    {        
        if (GameManager.hud.abilityPanel.activeSelf) { return; }
        
        if(canActivate && InputControl.GetButtonDown("Interact") 
            && !DialogManager.instance.dialogueBox.activeInHierarchy && GameManager.menu.gameObject.activeSelf == false)
        {
//            GameManager.IsDialogueActive = true;
            DialogManager.instance.ShowDialog(lines, isPerson, pitchLevel);
            DialogManager.instance.SetPortrait(portraitImage);
            DialogManager.instance.SetName(npcName);
        }   
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag(Tag.PlayerTag))
        {
            canActivate = true;
            buttonSprite.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag(Tag.PlayerTag))
        {
            canActivate = false;
            buttonSprite.SetActive(false);
            GameManager.Instance.peekDisabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (this.enabled == false) { return; }

        if (_animator != null)
        {
            switch (KeyBindings.CurrentProfile)
            {
                case KeyBindings.Profile.Profile1:
                    _animator.SetBool("E",    false);
                    _animator.SetBool("Down", true);
                    break;
                case KeyBindings.Profile.Profile2:
                    _animator.SetBool("Down", false);
                    _animator.SetBool("E",    true);
                    break;
                case KeyBindings.Profile.Profile3:
                    _animator.SetBool("E",    false);
                    _animator.SetBool("Down", true);
                    break;
            }
        }
        
        buttonXSize = Mathf.Abs(buttonSprite.transform.localScale.x);
        if (this.gameObject.transform.localScale.x > 0f)
        {
            buttonSprite.transform.localScale = new Vector3(buttonXSize, buttonSprite.transform.localScale.y, buttonSprite.transform.localScale.z);
        }
        else if (this.gameObject.transform.localScale.x < 0f)
        {
            buttonSprite.transform.localScale = new Vector3(-buttonXSize, buttonSprite.transform.localScale.y, buttonSprite.transform.localScale.z);
        }

        if (player.isDeactivated) { return; }

        if(other.CompareTag(Tag.PlayerTag))
        {
            if(player.GetComponent<Animator>().GetBool("isRolling")) { return; }
            if(player.GetComponent<Animator>().GetBool("isClimbing")) { return; }
            if(player.GetComponent<Animator>().GetBool("isSliding")) { return; }
            if(player.GetComponent<Animator>().GetBool("isJumping")) { return; }
            
            GameManager.Instance.peekDisabled = true;
        }
    }
}
