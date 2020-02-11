using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconChanger : MonoBehaviour
{
    [Header("Home Scene")]
    [SerializeField]
    private SpriteRenderer leftKey;

    [SerializeField] private SpriteRenderer rightKey;
    [SerializeField] private SpriteRenderer pauseKey;
    [SerializeField] private SpriteRenderer interactKey;

    [Header("Tutorial Scene")]
    [SerializeField]
    private SpriteRenderer jumpKey;

    [SerializeField] private SpriteRenderer lookDownKey;
    [SerializeField] private SpriteRenderer dashKey;
    [SerializeField] private SpriteRenderer shootKey;
    [SerializeField] private SpriteRenderer climbKey;
    [SerializeField] private SpriteRenderer dashKey2;
    [SerializeField] private SpriteRenderer dashKey3;
    [SerializeField] private SpriteRenderer recallKey;

    [SerializeField] private TextMeshPro upgradeText;

    [Header("Puzzle Scene")]
    [SerializeField]
    private SpriteRenderer bluePortalKey;

    [SerializeField] private SpriteRenderer redPortalKey;

    [Header("Sprites Home Scene")]
    [SerializeField]
    private Sprite[] leftKeyProfiles;

    [SerializeField] private Sprite[] rigthKeyProfiles;
    [SerializeField] private Sprite[] pauseKeyProfiles;
    [SerializeField] private Sprite[] interactKeyProfiles;

    [Header("Sprites Tutorial Scene")]
    [SerializeField]
    private Sprite[] jumpKeyProfiles;

    [SerializeField] private Sprite[] lookDownKeyProfiles;
    [SerializeField] private Sprite[] dashKeyProfiles;
    [SerializeField] private Sprite[] shootKeyProfiles;
    [SerializeField] private Sprite[] climgKeyProfiles;
    [SerializeField] private Sprite[] recallKeyProfiles;

    [SerializeField] private string[] textProfiles;

    [Header("Sprites Puzzle Scene")]
    [SerializeField]
    private Sprite[] bluePortalKeyProfiles;

    [SerializeField] private Sprite[] redPortalKeyProfiles;

    private void Update ()
    {
        switch (KeyBindings.CurrentProfile)
        {
            case KeyBindings.Profile.Profile1:
                if (leftKey     != null) leftKey.sprite     = leftKeyProfiles[0];
                if (rightKey    != null) rightKey.sprite    = rigthKeyProfiles[0];
                if (pauseKey    != null) pauseKey.sprite    = pauseKeyProfiles[0];
                if (interactKey != null) interactKey.sprite = interactKeyProfiles[0];

                if (jumpKey     != null) jumpKey.sprite     = jumpKeyProfiles[0];
                if (lookDownKey != null) lookDownKey.sprite = lookDownKeyProfiles[0];
                if (dashKey     != null) dashKey.sprite     = dashKeyProfiles[0];
                if (shootKey    != null) shootKey.sprite    = shootKeyProfiles[0];
                if (climbKey    != null) climbKey.sprite    = climgKeyProfiles[0];
                if (dashKey2    != null) dashKey2.sprite    = dashKeyProfiles[0];
                if (dashKey3    != null) dashKey3.sprite    = dashKeyProfiles[0];
                if (recallKey   != null) recallKey.sprite   = recallKeyProfiles[0];

                if (upgradeText != null) upgradeText.text = textProfiles[0];

                if (bluePortalKey != null) bluePortalKey.sprite = bluePortalKeyProfiles[0];
                if (redPortalKey  != null) redPortalKey.sprite  = redPortalKeyProfiles[0];
                break;
            case KeyBindings.Profile.Profile2:
                if (leftKey     != null) leftKey.sprite     = leftKeyProfiles[1];
                if (rightKey    != null) rightKey.sprite    = rigthKeyProfiles[1];
                if (pauseKey    != null) pauseKey.sprite    = pauseKeyProfiles[1];
                if (interactKey != null) interactKey.sprite = interactKeyProfiles[1];

                if (jumpKey     != null) jumpKey.sprite     = jumpKeyProfiles[1];
                if (lookDownKey != null) lookDownKey.sprite = lookDownKeyProfiles[1];
                if (dashKey     != null) dashKey.sprite     = dashKeyProfiles[1];
                if (shootKey    != null) shootKey.sprite    = shootKeyProfiles[1];
                if (climbKey    != null) climbKey.sprite    = climgKeyProfiles[1];
                if (dashKey2    != null) dashKey2.sprite    = dashKeyProfiles[1];
                if (dashKey3    != null) dashKey3.sprite    = dashKeyProfiles[1];
                if (recallKey   != null) recallKey.sprite   = recallKeyProfiles[1];

                if (upgradeText != null) upgradeText.text = textProfiles[1];

                if (bluePortalKey != null) bluePortalKey.sprite = bluePortalKeyProfiles[1];
                if (redPortalKey  != null) redPortalKey.sprite  = redPortalKeyProfiles[1];
                break;
            case KeyBindings.Profile.Profile3:
                if (leftKey     != null) leftKey.sprite     = leftKeyProfiles[2];
                if (rightKey    != null) rightKey.sprite    = rigthKeyProfiles[2];
                if (pauseKey    != null) pauseKey.sprite    = pauseKeyProfiles[2];
                if (interactKey != null) interactKey.sprite = interactKeyProfiles[2];

                if (jumpKey     != null) jumpKey.sprite     = jumpKeyProfiles[2];
                if (lookDownKey != null) lookDownKey.sprite = lookDownKeyProfiles[2];
                if (dashKey     != null) dashKey.sprite     = dashKeyProfiles[2];
                if (shootKey    != null) shootKey.sprite    = shootKeyProfiles[2];
                if (climbKey    != null) climbKey.sprite    = climgKeyProfiles[2];
                if (dashKey2    != null) dashKey2.sprite    = dashKeyProfiles[2];
                if (dashKey3    != null) dashKey3.sprite    = dashKeyProfiles[2];
                if (recallKey   != null) recallKey.sprite   = recallKeyProfiles[2];

                if (upgradeText != null) upgradeText.text = textProfiles[2];

                if (bluePortalKey != null) bluePortalKey.sprite = bluePortalKeyProfiles[2];
                if (redPortalKey  != null) redPortalKey.sprite  = redPortalKeyProfiles[2];
                break;
        }
    }
}