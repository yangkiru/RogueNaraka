using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableBullet : MonoBehaviour {

    [SerializeField]
    float speed;
    [SerializeField]
    float accel;
    [SerializeField]
    Vector3 velocity;
    [SerializeField]
    Space space;

    public void SetVelocity(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    public void SetSpace(Space space)
    {
        this.space = space;
    }

    private void Update()
    {
        transform.Translate(velocity * Time.deltaTime, space);
    }
}
