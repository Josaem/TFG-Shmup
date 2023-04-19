using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBullet : ProyectileBehavior
{
    [SerializeField]
    protected float _timeToStop = 2;
    [SerializeField]
    protected float _timeToDieAfterStop = 0.2f;

    protected override void Move()
    {
        rb.velocity = transform.up.normalized * _speed;

        Invoke(nameof(StopMovement), _timeToStop);
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;

        Invoke(nameof(Die), _timeToDieAfterStop);
    }
}
