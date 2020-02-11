using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class InteractWithShop : MonoBehaviour
{
    [SerializeField] private GameObject buttonSprite;
    [SerializeField] private GameObject upgradePanel;

    [Header("Ability/Bag Panel Code")]
    [SerializeField]
    private bool isAbilityPanel;

    [SerializeField] private GameObject abilityPanel;
    [SerializeField] private GameObject archeyPanel;

    private enum ShopState
    {
        Enabled,
        Disabled
    };

    private ShopState shopState = ShopState.Disabled;

    private bool   canActivate;
    private Player player;

    private Animator _animator;

    private void Start () { _animator = buttonSprite.GetComponent<Animator>(); }

    private void Update ()
    {
        if (GameManager.menu.gameObject.activeSelf) { return; }

        if (GameManager.hud.abilityPanel.activeSelf) { return; }

        if (canActivate)
        {
            if (InputControl.GetButtonDown("Interact"))
                if (shopState == ShopState.Disabled)
                    OpenShop();

            if (shopState == ShopState.Enabled)
                if (InputControl.GetButtonDown("Close"))
                    CloseShop();
        }
    }

    private void OpenShop ()
    {
        GameManager.IsPanelOpen = true;

        if (isAbilityPanel) archeyPanel.SetActive(true);

        upgradePanel.SetActive(true);
        upgradePanel.GetComponent<RestoreFocus>().SelectButton();
        shopState            = ShopState.Enabled;
        player.isDeactivated = true;
    }

    private void CloseShop ()
    {
        GameManager.IsPanelOpen = false;

        if (isAbilityPanel)
        {
            if (abilityPanel != null) abilityPanel.SetActive(false);
            archeyPanel.SetActive(false);
        }

        upgradePanel.SetActive(false);
        shopState            = ShopState.Disabled;
        player.isDeactivated = false;
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
        if (other.CompareTag(Tag.PlayerTag))
        {
            buttonSprite.SetActive(false);
            canActivate = false;
            GameManager.Instance.peekDisabled = false;
        }
    }
}