using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodStainRandomizer : MonoBehaviour
{
    [SerializeField] private Sprite[] bloodStains;
    [SerializeField] private float    minBloodStainSize = .05f;
    [SerializeField] private float    maxBloodStainSize = .1f;
    [SerializeField] private float    yOffset           = .5f;
    [SerializeField] private float    fadingTimer       = .2f;
    [SerializeField] private float    fadingDelay       = 30f;

    [Header ("Rotation Values")] [SerializeField]
    private float maxRotX = 50f;

    [SerializeField] private float minRotX = 0f;
    [SerializeField] private float maxRotY = 50f;
    [SerializeField] private float minRotY = 0f;
    [SerializeField] private float maxRotZ = 180f;
    [SerializeField] private float minRotZ = -180f;

    [Header ("Random Colors")]
    [SerializeField] private Color[] colors;

    [Header ("Sorting Order Min/Max")]
    [SerializeField] private int minLayerOrder = -200;

    [SerializeField] private int maxLayerOrder = -100;

    private SpriteRenderer spriteRenderer;
    private Color          color, tempColor;

    private int   randomBloodStain, randomColor, randomLayerOrder;
    private float randomXSize,      randomYSize;
    private float randomRotX,       randomRotY, randomRotZ;
    private float alphaValue;

    private void Awake ()
    {
        randomColor      = Mathf.RoundToInt (Random.Range (0,             colors.Length));
        randomBloodStain = Mathf.RoundToInt (Random.Range (0,             bloodStains.Length));
        randomLayerOrder = Mathf.RoundToInt (Random.Range (minLayerOrder, maxLayerOrder));
        randomXSize      = Random.Range (minBloodStainSize, maxBloodStainSize);
        randomYSize      = Random.Range (minBloodStainSize, maxBloodStainSize);
        randomRotX       = Random.Range (minRotX,           maxRotX);
        randomRotY       = Random.Range (minRotY,           maxRotY);
        randomRotZ       = Random.Range (minRotZ,           maxRotZ);

        transform.rotation   = Quaternion.Euler (randomRotX, randomRotY, randomRotZ);
        transform.localScale = new Vector3 (randomXSize, randomYSize, 1f);
        transform.position =
            new Vector3 (transform.position.x, transform.position.y + yOffset, +transform.position.z);

        spriteRenderer = GetComponent<SpriteRenderer> ();
        color          = spriteRenderer.material.color;

        spriteRenderer.color        = colors[randomColor];
        spriteRenderer.sprite       = bloodStains[randomBloodStain];
        spriteRenderer.sortingOrder = randomLayerOrder;

        //StartCoroutine (Fade ());
    }

    private IEnumerator Fade ()
    {
        alphaValue = color.a;
        tempColor  = color;

        tempColor.a = alphaValue;

        yield return new WaitForSeconds (fadingDelay);

        for ( float i = 0f; tempColor.a >= 0f; i += 0.1f )
        {
            tempColor.a                   -= 0.01f;
            spriteRenderer.material.color =  tempColor;

            yield return new WaitForSeconds (fadingTimer);
        }

        Destroy (gameObject);
    }
}