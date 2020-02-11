using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableDoorUnlocker : MonoBehaviour
{
    [Header("Level Exit Doors")]
    [SerializeField]
    private LevelExit[] exitDoors;

    [Header("Gates")]
    [SerializeField]
    private Animator[] gates;

    [Header("Dialogue")]
    [SerializeField]
    private DialogActivator dialogue;

    private Collectable _collectable;

    private void Start ()
    {
        _collectable = GetComponent<Collectable>();
        LockDoors();
    }

    private void LockDoors ()
    {
        if (_collectable.itemState == Collectable.ItemState.Collected)
        {
            if (dialogue) dialogue.enabled = false;
            return;
        }
        
        if (exitDoors.Length != 0)
            foreach (LevelExit door in exitDoors)
                door.enabled = false;
    }

    public void UnlockDoors ()
    {
        if (exitDoors.Length != 0)
            foreach (LevelExit door in exitDoors)
                door.enabled = true;

        if (gates.Length != 0)
            foreach (Animator gate in gates)
                gate.SetBool("isLocked", false);

        if (dialogue) dialogue.enabled = false;
    }
}