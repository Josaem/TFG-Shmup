using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotativeBullet : ProyectileBehavior
{
    [SerializeField]
    protected float _rot = 0;

    private void FixedUpdate()
    {
        transform.Rotate(0, 0, _rot * Time.deltaTime);
        rb.velocity = transform.up.normalized * _speed;
    }
}
