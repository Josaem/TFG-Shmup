using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class SteppingLockBullet : ProyectileBehavior
{
    [SerializeField]
    private int _steps;
    [SerializeField]
    private float _startingDistance;
    [SerializeField]
    private float _stepDistance;
    [SerializeField]
    private float _stepTime;

    private int _stepIndex = 0;

    public override void Move(float speed)
    {
        _speed = speed;
        rb.velocity = transform.up * _speed;

        float timeToStep = _startingDistance / _speed;
        Invoke(nameof(StepStop), timeToStep);
    }

    private void StepMov()
    {
        if (_stepIndex < _steps)
        {
            Transform player = FindObjectOfType<PlayerController>().transform;
            transform.up = player.position - transform.position;

            float timeToStep = _stepDistance / _speed;
            Invoke(nameof(StepStop), timeToStep);
        }

        rb.velocity = transform.up * _speed;
    }

    private void StepStop()
    {        
        rb.velocity = Vector2.zero;
        _stepIndex++;
        Invoke(nameof(StepMov), _stepTime);
    }

}
