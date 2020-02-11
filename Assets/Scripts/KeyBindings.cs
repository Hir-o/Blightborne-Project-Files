using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;

public class KeyBindings : MonoBehaviour
{
    public enum Profile
    {
        Profile1,
        Profile2,
        Profile3
    }

    public static Profile CurrentProfile = Profile.Profile1;

    [SerializeField] private TextMeshProUGUI dialogueContinueText;
    [SerializeField] private TextMeshProUGUI upgradeKey;
    [SerializeField] private TextMeshProUGUI nextSkill, prevSkill;
    [SerializeField] private TextMeshProUGUI bluePortal, redPortal;

    [Header("Images")]
    [SerializeField]
    private Image keyboardImage;
    [SerializeField]
    private Image selectedProfileImage;
    [SerializeField]
    private Sprite[] _sprites;

    [SerializeField] private TextMeshProUGUI profileInUseText;
    
    public void Profile1 ()
    {
        Controls.buttons.up           = InputControl.setKey("Up",                 KeyCode.UpArrow, KeyCode.None);
        Controls.buttons.down         = InputControl.setKey("Down",               KeyCode.DownArrow, KeyCode.None);
        Controls.buttons.left         = InputControl.setKey("Left",               KeyCode.LeftArrow, KeyCode.None);
        Controls.buttons.right        = InputControl.setKey("Right",              KeyCode.RightArrow, KeyCode.None);
        Controls.buttons.jump         = InputControl.setKey("Jump",               KeyCode.UpArrow, KeyCode.None);
        Controls.buttons.roll         = InputControl.setKey("Roll",               KeyCode.X, KeyCode.None);
        Controls.buttons.shoot        = InputControl.setKey("Fire1",              KeyCode.Z, KeyCode.None);
        Controls.buttons.abilityPanel = InputControl.setKey("Ability Panel",      KeyCode.F, KeyCode.None);
        Controls.buttons.peek         = InputControl.setKey("Peek",               KeyCode.DownArrow, KeyCode.None);
        Controls.buttons.menu         = InputControl.setKey("Menu",               KeyCode.P, KeyCode.None);
        Controls.buttons.interact     = InputControl.setKey("Interact",           KeyCode.DownArrow, KeyCode.None);
        Controls.buttons.next         = InputControl.setKey("Next",               KeyCode.Z, KeyCode.None);
        Controls.buttons.close        = InputControl.setKey("Close",              KeyCode.X, KeyCode.None);
        Controls.buttons.returnToBase = InputControl.setKey("Return To Base",     KeyCode.B, KeyCode.None);
        Controls.buttons.nextSkill    = InputControl.setKey("Scroll Skill Right", KeyCode.S, KeyCode.None);
        Controls.buttons.prevSkill    = InputControl.setKey("Scroll Skill Left",  KeyCode.A, KeyCode.None);
        Controls.buttons.vertical     = InputControl.setKey("Vertical",           KeyCode.UpArrow, KeyCode.None);
        Controls.buttons.bluePortal   = InputControl.setKey("Blue Portal",        KeyCode.C, KeyCode.None);
        Controls.buttons.redPortal    = InputControl.setKey("Red Portal",         KeyCode.V, KeyCode.None);

        Controls.axes.vertical   = InputControl.setAxis("Vertical",   Controls.buttons.down, Controls.buttons.up);
        Controls.axes.horizontal = InputControl.setAxis("Horizontal", Controls.buttons.left, Controls.buttons.right);
        
        CurrentProfile = Profile.Profile1;
        dialogueContinueText.text = "(z) Continue >>";
        upgradeKey.text = "F";
        nextSkill.text = "S";
        prevSkill.text = "A";
        bluePortal.text = "C";
        redPortal.text  = "V";
        
        keyboardImage.sprite = _sprites[0];
        profileInUseText.text = "Profile 1 in use";
    }

    public void Profile2 ()
    {
        Controls.buttons.up           = InputControl.setKey("Up",                 KeyCode.W, KeyCode.None);
        Controls.buttons.down         = InputControl.setKey("Down",               KeyCode.S, KeyCode.None);
        Controls.buttons.left         = InputControl.setKey("Left",               KeyCode.A, KeyCode.None);
        Controls.buttons.right        = InputControl.setKey("Right",              KeyCode.D, KeyCode.None);
        Controls.buttons.jump         = InputControl.setKey("Jump",               KeyCode.W, KeyCode.None);
        Controls.buttons.roll         = InputControl.setKey("Roll",               KeyCode.K, KeyCode.None);
        Controls.buttons.shoot        = InputControl.setKey("Fire1",              KeyCode.J, KeyCode.None);
        Controls.buttons.abilityPanel = InputControl.setKey("Ability Panel",      KeyCode.U, KeyCode.None);
        Controls.buttons.peek         = InputControl.setKey("Peek",               KeyCode.S, KeyCode.None);
        Controls.buttons.menu         = InputControl.setKey("Menu",               KeyCode.P, KeyCode.None);
        Controls.buttons.interact     = InputControl.setKey("Interact",           KeyCode.S, KeyCode.None);
        Controls.buttons.next         = InputControl.setKey("Next",               KeyCode.J, KeyCode.None);
        Controls.buttons.close        = InputControl.setKey("Close",              KeyCode.K, KeyCode.None);
        Controls.buttons.returnToBase = InputControl.setKey("Return To Base",     KeyCode.B, KeyCode.None);
        Controls.buttons.nextSkill    = InputControl.setKey("Scroll Skill Right", KeyCode.E, KeyCode.None);
        Controls.buttons.prevSkill    = InputControl.setKey("Scroll Skill Left",  KeyCode.Q, KeyCode.None);
        Controls.buttons.vertical     = InputControl.setKey("Vertical",           KeyCode.W, KeyCode.None);
        Controls.buttons.bluePortal   = InputControl.setKey("Blue Portal",        KeyCode.I, KeyCode.None);
        Controls.buttons.redPortal    = InputControl.setKey("Red Portal",         KeyCode.O, KeyCode.None);

        Controls.axes.vertical   = InputControl.setAxis("Vertical",   Controls.buttons.down, Controls.buttons.up);
        Controls.axes.horizontal = InputControl.setAxis("Horizontal", Controls.buttons.left, Controls.buttons.right);
        
        CurrentProfile = Profile.Profile2;
        dialogueContinueText.text = "(j) Continue >>";
        upgradeKey.text = "U";
        nextSkill.text = "E";
        prevSkill.text = "Q";
        bluePortal.text = "I";
        redPortal.text = "O";

        keyboardImage.sprite = _sprites[1];
        profileInUseText.text = "Profile 2 in use";
    }

    public void Profile3 ()
    {
        Controls.buttons.up           = InputControl.setKey("Up",                 KeyCode.UpArrow, KeyCode.None);
        Controls.buttons.down         = InputControl.setKey("Down",               KeyCode.DownArrow, KeyCode.None);
        Controls.buttons.left         = InputControl.setKey("Left",               KeyCode.LeftArrow, KeyCode.None);
        Controls.buttons.right        = InputControl.setKey("Right",              KeyCode.RightArrow, KeyCode.None);
        Controls.buttons.jump         = InputControl.setKey("Jump",               KeyCode.UpArrow, KeyCode.None);
        Controls.buttons.roll         = InputControl.setKey("Roll",               KeyCode.S, KeyCode.None);
        Controls.buttons.shoot        = InputControl.setKey("Fire1",              KeyCode.A, KeyCode.None);
        Controls.buttons.abilityPanel = InputControl.setKey("Ability Panel",      KeyCode.F, KeyCode.None);
        Controls.buttons.peek         = InputControl.setKey("Peek",               KeyCode.DownArrow, KeyCode.None);
        Controls.buttons.menu         = InputControl.setKey("Menu",               KeyCode.P, KeyCode.None);
        Controls.buttons.interact     = InputControl.setKey("Interact",           KeyCode.DownArrow, KeyCode.None);
        Controls.buttons.next         = InputControl.setKey("Next",               KeyCode.A, KeyCode.None);
        Controls.buttons.close        = InputControl.setKey("Close",              KeyCode.S, KeyCode.None);
        Controls.buttons.returnToBase = InputControl.setKey("Return To Base",     KeyCode.B, KeyCode.None);
        Controls.buttons.nextSkill    = InputControl.setKey("Scroll Skill Right", KeyCode.W, KeyCode.None);
        Controls.buttons.prevSkill    = InputControl.setKey("Scroll Skill Left",  KeyCode.Q, KeyCode.None);
        Controls.buttons.vertical     = InputControl.setKey("Vertical",           KeyCode.UpArrow, KeyCode.None);
        Controls.buttons.bluePortal   = InputControl.setKey("Blue Portal",        KeyCode.X, KeyCode.None);
        Controls.buttons.redPortal    = InputControl.setKey("Red Portal",         KeyCode.C, KeyCode.None);

        Controls.axes.vertical   = InputControl.setAxis("Vertical",   Controls.buttons.down, Controls.buttons.up);
        Controls.axes.horizontal = InputControl.setAxis("Horizontal", Controls.buttons.left, Controls.buttons.right);
        
        CurrentProfile = Profile.Profile3;
        dialogueContinueText.text = "(a) Continue >>";
        upgradeKey.text = "F";
        nextSkill.text = "W";
        prevSkill.text = "Q";
        bluePortal.text = "X";
        redPortal.text  = "C";
        
        keyboardImage.sprite = _sprites[2];
        profileInUseText.text = "Profile 3 in use";
    }

    public void KeyboardProfile1 ()
    {
        selectedProfileImage.sprite = _sprites[0];
        keyboardImage.gameObject.SetActive(false);
    }

    public void KeyboardProfile2 ()
    {
        selectedProfileImage.sprite = _sprites[1];
        keyboardImage.gameObject.SetActive(false);
    }

    public void ResetKeyboardImage ()
    {
        keyboardImage.gameObject.SetActive(true);
    }
}