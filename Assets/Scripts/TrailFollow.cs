using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailFollow : MonoBehaviour {

    [SerializeField] private Transform target;

    private Vector2 stillVelocity = new Vector2(0f, 0f);

    private void FixedUpdate()
    {
        if(target == null || target.GetComponent<Rigidbody2D>().velocity == stillVelocity)
            GetComponent<Rigidbody2D>().velocity = stillVelocity;
    }

    public void SetTarget(Transform arrow)
    {
        target = arrow;
    }

    public void ShootTrail(Vector2 trailVelocity)
    {
        GetComponent<Rigidbody2D>().velocity += trailVelocity;
    }

}
