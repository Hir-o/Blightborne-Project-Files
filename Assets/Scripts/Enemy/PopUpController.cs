using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpController : MonoBehaviour
{
    [SerializeField] private GameObject hitPopup;
    [SerializeField] private GameObject enemyYellPopup;
    [SerializeField] private GameObject noStaminaPopUp;
    [SerializeField] private GameObject noHpPopup;
    [SerializeField] private Vector3    spawnLocationOffset;

    private int randomNumber;

    public void EnablePopUp()
    {
        randomNumber = Random.Range(0, 10);

        if (randomNumber < 5) Instantiate(hitPopup, transform.position + spawnLocationOffset, Quaternion.identity);
    }

    public void BattleCryPopUp() // fired from animation (run) event
    {
        randomNumber = Random.Range(0, 20);
        if (randomNumber < 2)
            Instantiate(enemyYellPopup, transform.position + spawnLocationOffset, Quaternion.identity);
    }

    public void BatRoarPopUp()
    {
        Instantiate(enemyYellPopup, transform.position + spawnLocationOffset, Quaternion.identity);
    }

    public void FinalBossRoarPopUp()
    {
        Instantiate(enemyYellPopup, transform.position + spawnLocationOffset, Quaternion.identity);
    }

    public void NoStaminaPopUp()
    {
        Instantiate(noStaminaPopUp, transform.position + spawnLocationOffset, Quaternion.identity);
    }

    public void NoHpPopup()
    {
        Instantiate(noHpPopup, transform.position + spawnLocationOffset, Quaternion.identity);
    }
}