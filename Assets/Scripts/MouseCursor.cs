using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MouseCursor : MonoBehaviour {

    //currentpos stores the cursor position
    //Vector2 cursorPosition;

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    //enable if using mouse sprite

    // private void Update()
    // {
    //     cursorPosition = Camera.main.ScreenToWorldPoint(CrossPlatformInputManager.mousePosition);
    //     transform.position = cursorPosition;
    // }
}
