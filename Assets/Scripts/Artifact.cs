using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Artifact : MonoBehaviour
{
    [SerializeField] private Vector3 offset;

    [SerializeField] private float speed             = 5f;
    [SerializeField] private float followPlayerTimer = 3f;
    [SerializeField] private float openDoorTimer     = 6f;

    [SerializeField] private GameObject door;

    private SpriteRenderer _spriteRenderer;

    private bool    followPlayer;
    private Vector2 distance;
    private Vector3 tempVector;

    private void Start ()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (gameObject.activeSelf) FollowPlayer();
    }

    public void FollowPlayer () { Invoke(nameof(GoToPlayer), followPlayerTimer); }

    public void EnableDoor () { Invoke(nameof(OpenDoor), openDoorTimer); }

    private void Update ()
    {
        distance = GameManager.player.transform.position - transform.position;

        if (distance.x < 0)
        {
            _spriteRenderer.flipX = true;
            tempVector            = new Vector3(offset.x, offset.y, offset.z);
        }
        else
        {
            _spriteRenderer.flipX = false;
            tempVector            = new Vector3(-offset.x, offset.y, offset.z);
        }

        if (followPlayer)
            transform.position = Vector2.Lerp(transform.position, GameManager.player.transform.position + tempVector,
                                              speed * Time.deltaTime);
    }

    private void GoToPlayer () { followPlayer = true; }

    private void OpenDoor () { door.SetActive(true); }
}