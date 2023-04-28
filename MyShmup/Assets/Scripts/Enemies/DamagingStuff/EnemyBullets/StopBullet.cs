using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBullet : ProyectileBehavior
{
    [SerializeField]
    protected float _timeToStop = 2;
    [SerializeField]
    protected float _timeToDieAfterStop = 0.2f;

    public override void Move(float speed)
    {
        _speed = speed;
        rb.velocity = transform.up * _speed;

        Invoke(nameof(StopMovement), _timeToStop);
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;

        Invoke(nameof(Die), _timeToDieAfterStop);
    }
}
