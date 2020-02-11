using UnityEngine;
using DG.Tweening;

public class GhostTrail : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Transform ghostsParent;
    public Color trailColor;
    public Color fadeColor;
    public float ghostInterval;
    public float fadeTime;

    public bool isFlipped;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ShowGhost()
    {
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            if (GameManager.player.transform.localScale.x < 0) isFlipped = true;
            else if (GameManager.player.transform.localScale.x > 0) isFlipped = false;
            
            Transform currentGhost = ghostsParent.GetChild(i);
            s.AppendCallback(()=> currentGhost.position = GameManager.player.transform.position);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = isFlipped);
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
            s.AppendCallback(() => FadeSprite(currentGhost));
            s.AppendInterval(ghostInterval);
        }
    }
    
    public void ShowTutorialDashGhost(Vector3 position)
    {
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);
            s.AppendCallback(()=> currentGhost.position                              = position);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = isFlipped);
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
            s.AppendCallback(() => FadeSprite(currentGhost));
            s.AppendInterval(ghostInterval);
        }
    }
    
    public void ShowGhostFinalBoss(Vector3 position)
    {
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < ghostsParent.childCount; i++)
        {
            Transform currentGhost = ghostsParent.GetChild(i);
            s.AppendCallback(()=> currentGhost.position                              = position);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = isFlipped);
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(trailColor, 0));
            s.AppendCallback(() => FadeSprite(currentGhost));
            s.AppendInterval(ghostInterval);
        }
    }

    public void FadeSprite(Transform current)
    {
        current.GetComponent<SpriteRenderer>().material.DOKill();
        current.GetComponent<SpriteRenderer>().material.DOColor(fadeColor, fadeTime);
    }

}
