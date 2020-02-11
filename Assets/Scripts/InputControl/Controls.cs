using UnityEngine;
using System.Collections.ObjectModel;


/// <summary>
/// <see cref="Controls"/> is a set of user defined buttons and axes. It is better to store this file somewhere in your project.
/// </summary>
public static class Controls
{
    /// <summary>
    /// <see cref="Buttons"/> is a set of user defined buttons.
    /// </summary>
    public struct Buttons
    {
        public KeyMapping up;
        public KeyMapping down;
        public KeyMapping left;
        public KeyMapping right;
        public KeyMapping jump;
        public KeyMapping roll;
        public KeyMapping shoot;
        public KeyMapping peek;
        public KeyMapping menu;
        public KeyMapping abilityPanel;
        public KeyMapping interact;
        public KeyMapping next;
        public KeyMapping close;
        public KeyMapping returnToBase;
        public KeyMapping nextSkill;
        public KeyMapping prevSkill;
        public KeyMapping vertical;
        public KeyMapping horizontal;
        public KeyMapping submit;
        public KeyMapping bluePortal;
        public KeyMapping redPortal;
    }

    /// <summary>
    /// <see cref="Axes"/> is a set of user defined axes.
    /// </summary>
    public struct Axes
    {
        public Axis vertical;
        public Axis horizontal;
    }


    /// <summary>
    /// Set of buttons.
    /// </summary>
    public static Buttons buttons;

    /// <summary>
    /// Set of axes.
    /// </summary>
    public static Axes axes;

    /// <summary>
    /// Initializes the <see cref="Controls"/> class.
    /// </summary>
    static Controls ()
    {
        buttons.up           = InputControl.setKey("Up",                 KeyCode.UpArrow, KeyCode.None, new JoystickInput(JoystickAxis.Axis2Negative));
        buttons.down         = InputControl.setKey("Down",               KeyCode.DownArrow, KeyCode.None, new JoystickInput(JoystickAxis.Axis2Positive));
        buttons.left         = InputControl.setKey("Left",               KeyCode.LeftArrow, KeyCode.None, new JoystickInput(JoystickAxis.Axis1Negative));
        buttons.right        = InputControl.setKey("Right",              KeyCode.RightArrow, KeyCode.None, new JoystickInput(JoystickAxis.Axis1Positive));
        buttons.jump         = InputControl.setKey("Jump",               KeyCode.UpArrow, KeyCode.None, new JoystickInput(JoystickButton.Button4));
        buttons.roll         = InputControl.setKey("Roll",               KeyCode.X, KeyCode.None);
        buttons.shoot        = InputControl.setKey("Fire1",              KeyCode.Z, KeyCode.None, new JoystickInput(JoystickButton.Button1));
        buttons.abilityPanel = InputControl.setKey("Ability Panel",      KeyCode.F, KeyCode.None);
        buttons.peek         = InputControl.setKey("Peek",               KeyCode.DownArrow, KeyCode.None, new JoystickInput(JoystickAxis.Axis4Positive));
        buttons.menu         = InputControl.setKey("Menu",               KeyCode.P, KeyCode.None);
        buttons.interact     = InputControl.setKey("Interact",           KeyCode.DownArrow, KeyCode.None);
        buttons.next         = InputControl.setKey("Next",               KeyCode.Z, KeyCode.None);
        buttons.close        = InputControl.setKey("Close",              KeyCode.X, KeyCode.None);
        buttons.returnToBase = InputControl.setKey("Return To Base",     KeyCode.B, KeyCode.None);
        buttons.nextSkill    = InputControl.setKey("Scroll Skill Right", KeyCode.S, KeyCode.None);
        buttons.prevSkill    = InputControl.setKey("Scroll Skill Left",  KeyCode.A, KeyCode.None);
        buttons.vertical     = InputControl.setKey("Vertical",           KeyCode.UpArrow, KeyCode.None);
        buttons.bluePortal   = InputControl.setKey("Blue Portal",        KeyCode.C, KeyCode.None);
        buttons.redPortal    = InputControl.setKey("Red Portal",         KeyCode.V, KeyCode.None);

        axes.vertical   = InputControl.setAxis("Vertical",   buttons.down, buttons.up);
        axes.horizontal = InputControl.setAxis("Horizontal", buttons.left, buttons.right);

        load();
    }

    /// <summary>
    /// Nothing. It just call static constructor if needed.
    /// </summary>
    public static void init ()
    {
        // Nothing. It just call static constructor if needed
    }

    /// <summary>
    /// Save controls.
    /// </summary>
    public static void save ()
    {
        // It is just an example. You may remove it or modify it if you want
        ReadOnlyCollection<KeyMapping> keys = InputControl.getKeysList();

        foreach (KeyMapping key in keys)
        {
            PlayerPrefs.SetString("Controls." + key.name + ".primary",   key.primaryInput.ToString());
            PlayerPrefs.SetString("Controls." + key.name + ".secondary", key.secondaryInput.ToString());
            PlayerPrefs.SetString("Controls." + key.name + ".third",     key.thirdInput.ToString());
        }

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load controls.
    /// </summary>
    public static void load ()
    {
        // It is just an example. You may remove it or modify it if you want
        ReadOnlyCollection<KeyMapping> keys = InputControl.getKeysList();

        foreach (KeyMapping key in keys)
        {
            string inputStr;

            inputStr = PlayerPrefs.GetString("Controls." + key.name + ".primary");

            if (inputStr != "") { key.primaryInput = customInputFromString(inputStr); }

            inputStr = PlayerPrefs.GetString("Controls." + key.name + ".secondary");

            if (inputStr != "") { key.secondaryInput = customInputFromString(inputStr); }

            inputStr = PlayerPrefs.GetString("Controls." + key.name + ".third");

            if (inputStr != "") { key.thirdInput = customInputFromString(inputStr); }
        }
    }

    /// <summary>
    /// Converts string representation of CustomInput to CustomInput.
    /// </summary>
    /// <returns>CustomInput from string.</returns>
    /// <param name="value">String representation of CustomInput.</param>
    private static CustomInput customInputFromString (string value)
    {
        CustomInput res;

        res = JoystickInput.FromString(value);

        if (res != null) { return res; }

        res = MouseInput.FromString(value);

        if (res != null) { return res; }

        res = KeyboardInput.FromString(value);

        if (res != null) { return res; }

        return null;
    }
}