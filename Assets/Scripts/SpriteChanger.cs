using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    private Image _image;

    private void OnEnable ()
    {
        _image = GetComponent<Image>();
        
        switch (KeyBindings.CurrentProfile)
        {
            case KeyBindings.Profile.Profile1:
                _image.sprite = _sprites[0];
                break;
            case KeyBindings.Profile.Profile2:
                _image.sprite = _sprites[1];
                break;
            case KeyBindings.Profile.Profile3:
                _image.sprite = _sprites[2];
                break;
        }
    }
}
