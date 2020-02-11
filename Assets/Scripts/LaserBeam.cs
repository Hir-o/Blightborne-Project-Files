using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    private Player       player;
    private Collider2D[] colliders;

    [Header("Laser attributes")]
    [SerializeField]
    private int laserDamage = 2;

    [Header("Player detection code")]
    [SerializeField]
    private float rangeX = 30f;

    [SerializeField] private float     rangeY = 1f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private Transform laserPosition;

    public bool dealDamageOnAnimation = false;

    private void Start () { InvokeRepeating("CheckForPlayer", .1f, .1f); }

    private void CheckForPlayer ()
    {
        colliders = Physics2D.OverlapBoxAll(laserPosition.position, new Vector2(rangeX, rangeY), 0f, playerMask);

        foreach (Collider2D coll in colliders)
        {
            if (coll.GetType() == typeof (CapsuleCollider2D))
            {
                if (player == null) player = coll.gameObject.GetComponent<Player>();

                if (dealDamageOnAnimation) player.HitByLaser(laserDamage);
            }
        }
    }

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(laserPosition.position, new Vector3(rangeX, rangeY, 1));
    }
}