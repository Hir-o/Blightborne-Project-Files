using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private Coin[]  coins;
    [SerializeField] private float   minCoins;
    [SerializeField] private float   maxCoins;
    [SerializeField] private Vector3 offset;

    private Collider2D collider;

    private                  int  randomNumber;
    [SerializeField] private bool collected = false;

    [Header("Used for saving chest state")]
    [SerializeField]
    private int chestIndex;

    [SerializeField] private enum ChestType
    {
        None,
        Iron,
        Gold
    }

    [SerializeField] private ChestType chestType = ChestType.None;

    private void Start ()
    {
        collider = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();

        if (ChestsController.chests[chestIndex] == 1)
        {
            collected = true;
            animator.SetTrigger("collect");
            collider.enabled = false;
        }
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        randomNumber = Mathf.RoundToInt(Random.Range(minCoins, maxCoins));
        if (collected) { return; }

        if (other.CompareTag(Tag.PlayerTag))
        {
            AudioController.Instance.ChestSFX();
            collected = true;

            for (int i = 0; i < randomNumber; i++)
                Instantiate(coins[Random.Range(0, coins.Length)], transform.position + offset, Quaternion.identity);

            animator.SetTrigger("collect");
            collider.enabled = false;

            ChestsController.chests[chestIndex] = 1;

            //Poki code
//            switch (chestType)
//            {
//                case ChestType.Iron:
//                    PokiUnitySDK.Instance.happyTime(0.2f);
//                    break;
//                case ChestType.Gold:
//                    PokiUnitySDK.Instance.happyTime(0.4f);
//                    break;
//            }
        }
    }
}