using UnityEngine;
using TMPro;

public class MessageFollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform platformsToFollow;
    [SerializeField] private float maxDistance = -10f;

    private TextMeshPro text;

    [SerializeField] private Vector3 offset = new Vector3(0f, 1f, 0f);

    private void Start () { text = GetComponent<TextMeshPro>(); }

    private void Update ()
    {
        //if (GameManager.player == null) { return; }
        transform.position = GameManager.player.transform.position + offset;

        if (PlayerStats.CheckpointPriority == 3)
        {
            gameObject.SetActive(false);
            return;
        }

        if (GameManager.player.transform.position.x - platformsToFollow.position.x < maxDistance)
            text.enabled = true;
        else
            text.enabled = false;
    }       
}
