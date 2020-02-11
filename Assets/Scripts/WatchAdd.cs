using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WatchAdd : MonoBehaviour
{
    private Player player;
    private bool   canActivate, isCollected;
    private float  buttonXSize;

    [SerializeField] private Animator _animator;

    [SerializeField] private GameObject buttonSprite;
    [SerializeField] private GameObject textMeshPro;

    [SerializeField] private int rewardCoins = 50;

    [SerializeField] private enum ChestNumber
    {
        None,
        FirstChest,
        SecondChest,
        ThirdChest,
        FourthChest
    }

    [SerializeField] private ChestNumber chestNumber = ChestNumber.None;

    private void Start ()
    {
        player    = GameManager.player;
        _animator = buttonSprite.GetComponent<Animator>();
        CheckForChestCollected();
    }

    private void Update ()
    {
        if (GameManager.hud.abilityPanel.activeSelf) { return; }

        if (isCollected)
        {
            if (buttonSprite.activeSelf) buttonSprite.SetActive(false);
            if (textMeshPro.activeSelf) textMeshPro.SetActive(false);
        }

        CheckForChestCollected();

        //POKI
//        if (canActivate && InputControl.GetButtonDown("Interact") && isCollected == false &&
//            PokiUnitySDK.Instance.adsBlocked()                                   == false
//            && GameManager.menu.gameObject.activeSelf                            == false) { WatchRewardAdd(); }
    }

    //Poki reward add
//    private void WatchRewardAdd ()
//    {
//        switch (chestNumber)
//        {
//            case ChestNumber.FirstChest:
//                PlayerStats.IsFirstAddChestCollected = true;
//                break;
//            case ChestNumber.SecondChest:
//                PlayerStats.IsSecondAddChestCollected = true;
//                break;
//            case ChestNumber.ThirdChest:
//                PlayerStats.IsThirdAddChestCollected = true;
//                break;
//            case ChestNumber.FourthChest:
//                PlayerStats.IsFourthAddChestCollected = true;
//                break;
//        }
//
//        AudioListener.volume = 0f;
//        isCollected         = true;
//
//        PokiUnitySDK.Instance.rewardedBreakCallBack = rewardedBreakComplete;
//        PokiUnitySDK.Instance.rewardedBreak();
//    }

//    public void rewardedBreakComplete (bool withReward)
//    {
//        AudioListener.volume =  1f;
//        PlayerStats.Coins   += rewardCoins;
//        EssentialObjects.UpdateCoinsStatic();
//    }
    //Poki reward add

    private void OnTriggerEnter2D (Collider2D other)
    {
        CheckForChestCollected();

        if (other.CompareTag(Tag.PlayerTag))
        {
            canActivate = true;
            buttonSprite.SetActive(true);
            textMeshPro.SetActive(true);
        }
    }

    private void OnTriggerExit2D (Collider2D other)
    {
        CheckForChestCollected();

        if (other.CompareTag(Tag.PlayerTag))
        {
            canActivate = false;
            buttonSprite.SetActive(false);
            textMeshPro.SetActive(false);
            GameManager.Instance.peekDisabled = false;
        }
    }

    private void OnTriggerStay2D (Collider2D other)
    {
        CheckForChestCollected();

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
            buttonSprite.transform.localScale = new Vector3(buttonXSize, buttonSprite.transform.localScale.y,
                                                            buttonSprite.transform.localScale.z);
        }
        else if (this.gameObject.transform.localScale.x < 0f)
        {
            buttonSprite.transform.localScale = new Vector3(-buttonXSize, buttonSprite.transform.localScale.y,
                                                            buttonSprite.transform.localScale.z);
        }

        if (player.isDeactivated) { return; }

        if (other.CompareTag(Tag.PlayerTag))
        {
            if (player.GetComponent<Animator>().GetBool("isRolling")) { return; }

            if (player.GetComponent<Animator>().GetBool("isClimbing")) { return; }

            if (player.GetComponent<Animator>().GetBool("isSliding")) { return; }

            if (player.GetComponent<Animator>().GetBool("isJumping")) { return; }

            GameManager.Instance.peekDisabled = true;
        }
    }

    private void CheckForChestCollected ()
    {
        if (chestNumber == ChestNumber.FirstChest && PlayerStats.IsFirstAddChestCollected)
        {
            GetComponent<Collider2D>().enabled = false;
            return;
        }

        if (chestNumber == ChestNumber.SecondChest && PlayerStats.IsSecondAddChestCollected)
        {
            GetComponent<Collider2D>().enabled = false;
            return;
        }

        if (chestNumber == ChestNumber.ThirdChest && PlayerStats.IsThirdAddChestCollected)
        {
            GetComponent<Collider2D>().enabled = false;
            return;
        }

        if (chestNumber == ChestNumber.FourthChest && PlayerStats.IsFourthAddChestCollected)
        {
            GetComponent<Collider2D>().enabled = false;
            return;
        }
    }
}