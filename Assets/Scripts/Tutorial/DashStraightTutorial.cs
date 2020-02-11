using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashStraightTutorial : MonoBehaviour
{
    [Header("Ghost Code")]
    [SerializeField]
    private GameObject _ghostGameObject;

    private GhostTrail _ghostTrail;

    [SerializeField] private GhostTrail _ghostTrailAttached;

    public void ShowGhostImages () // called by animation even
    {
        if (_ghostTrail == null) _ghostTrail = Instantiate(_ghostGameObject).GetComponent<GhostTrail>();

        _ghostTrail.ShowTutorialDashGhost(transform.position);
        _ghostTrailAttached.ShowTutorialDashGhost(transform.position);
    }
}
