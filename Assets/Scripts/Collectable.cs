using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Collectable : MonoBehaviour
{
    public enum ItemState
    {
        NotCollected,
        Collected
    };

    public ItemState itemState = ItemState.NotCollected;

    [SerializeField] private GameObject item;
    [SerializeField] private GameObject buttonSprite;

    [Header("Item Type")]
    [SerializeField]
    private bool isKeyBlue;
    [SerializeField] private bool isKeyRed;
    [SerializeField] private bool isGemBlue;
    [SerializeField] private bool isGemRed;

    private bool                    canActivate = false;
    private Player                  player;
    private BoxCollider2D           boxCollider;
    private CollectableDoorUnlocker doorUnlocker;

    private HUD hud;

    private Animator _animator;

    private void Start ()
    {
        _animator = buttonSprite.GetComponent<Animator>();
        hud         = FindObjectOfType<HUD>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (GetComponent<CollectableDoorUnlocker>()) doorUnlocker = GetComponent<CollectableDoorUnlocker>();

        if (PlayerStats.HasBlueKey && isKeyBlue)
        {
            item.SetActive(false);
            boxCollider.enabled = false;
            itemState = ItemState.Collected;
        }
        else if (PlayerStats.HasRedKey && isKeyRed)
        {
            item.SetActive(false);
            boxCollider.enabled = false;
            itemState = ItemState.Collected;
        }
        else if (PlayerStats.HasBlueGem && isGemBlue)
        {
            item.SetActive(false);
            boxCollider.enabled = false;
            itemState = ItemState.Collected;
        }
        else if (PlayerStats.HasRedGem && isGemRed)
        {
            item.SetActive(false);
            boxCollider.enabled = false;
            itemState = ItemState.Collected;
        }
    }

    private void Update ()
    {
        if (canActivate == true && itemState == ItemState.NotCollected)
            if (InputControl.GetButtonDown("Interact"))
                Collect();

        if (itemState == ItemState.Collected)
        {
            boxCollider.isTrigger = false;
            boxCollider.enabled   = false;
            buttonSprite.SetActive(false);
        }
    }

    public void Collect ()
    {
        item.SetActive(false);

        if (isKeyBlue)
            PlayerStats.HasBlueKey = true;
        else if (isKeyRed)
            PlayerStats.HasRedKey = true;
        else if (isGemBlue)
            PlayerStats.HasBlueGem               = true;
        else if (isGemRed) PlayerStats.HasRedGem = true;

        AudioController.Instance.CollectSFX();
        itemState = ItemState.Collected;

        if (doorUnlocker != null) doorUnlocker.UnlockDoors();

        hud.UpdateHUD();
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.PlayerTag))
        {
            player = other.gameObject.GetComponent<Player>();
            buttonSprite.SetActive(true);
            canActivate = true;
        }
    }

    private void OnTriggerStay2D (Collider2D other)
    {
        if (player == null) { return; }

        if (player.isDeactivated) { return; }
        
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

        if (other.CompareTag(Tag.PlayerTag))
        {
            if (player.GetComponent<Animator>().GetBool("isRolling")) { return; }

            if (player.GetComponent<Animator>().GetBool("isClimbing")) { return; }

            if (player.GetComponent<Animator>().GetBool("isSliding")) { return; }

            if (player.GetComponent<Animator>().GetBool("isJumping")) { return; }
            
            GameManager.Instance.peekDisabled = true;
        }
    }

    private void OnTriggerExit2D (Collider2D other)
    {
        if (other.gameObject.CompareTag(Tag.PlayerTag))
        {
            buttonSprite.SetActive(false);
            canActivate = false;
            GameManager.Instance.peekDisabled = false;
        }
    }
}