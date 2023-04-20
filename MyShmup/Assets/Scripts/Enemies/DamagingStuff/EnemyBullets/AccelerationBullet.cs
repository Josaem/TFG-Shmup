using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerationBullet : ProyectileBehavior
{
    [SerializeField]
    protected float _acceleration = 1f;

    protected override void Move()
    {
        rb.velocity = transform.up * _speed;
    }

    private void FixedUpdate()
    {
        _speed += Mathf.Max(_acceleration * 0.01f, 0);
        rb.velocity = transform.up * _speed;
    }

    protected override void SetDeath()
    {
        Invoke(nameof(Die), _maxDistance);
    }
}
