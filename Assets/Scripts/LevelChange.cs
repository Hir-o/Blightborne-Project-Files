using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChange : MonoBehaviour {

	[SerializeField] float sceneLoadTime = .2f;
    [SerializeField] float objectStillWaitTime = 0.05f;

    private Player player;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        player = collision.gameObject.GetComponent<Player>();

        StartCoroutine("PortalTransition");
    }

    IEnumerator PortalTransition()
    {
        yield return new WaitForSeconds(objectStillWaitTime);

        player.TeleportAnimation();

        yield return new WaitForSeconds(sceneLoadTime);

        LevelManager.LoadSceneWithName(LevelNameConstants.Level1, true);
    }
}
