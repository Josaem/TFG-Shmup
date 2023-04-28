using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationBullet : ProyectileBehavior
{
    [SerializeField]
    protected float _acceleration = 1f;

    private void FixedUpdate()
    {
        _speed += Mathf.Max(_acceleration * 0.01f, 0);
        rb.velocity = transform.up * _speed;
    }

    public override void SetDeath(float maxDistance)
    {
        Invoke(nameof(Die), maxDistance);
    }
}
