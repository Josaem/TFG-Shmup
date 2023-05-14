using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBullet : ProyectileBehavior
{
#if UNITY_EDITOR
    [Help("Dies after time max distance", UnityEditor.MessageType.None)]
#endif

    [SerializeField]
    protected float _timeToStop = 2;

    public override void Move(float speed)
    {
        _speed = speed;
        rb.velocity = transform.up * _speed;

        Invoke(nameof(StopMovement), _timeToStop);
    }

    private void StopMovement()
    {
        rb.velocity = Vector2.zero;
    }

    public override void SetDeath(float maxDistance)
    {
        Invoke(nameof(Die), maxDistance);
    }
}
