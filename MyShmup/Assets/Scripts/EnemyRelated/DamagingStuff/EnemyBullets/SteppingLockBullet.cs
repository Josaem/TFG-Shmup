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
    #if UNITY_EDITOR
    [Help("If rotation speed is 0 then it always locks the player", UnityEditor.MessageType.None)]
    #endif
    [SerializeField]
    private float _rotationSpeed;
    [SerializeField]
    private bool _lock = true;
    [SerializeField]
    private bool _lockOnLastStep;

    private int _stepIndex = 0;
    private bool _canRotate;
    private Transform _player;

    protected override void Start()
    {
        base.Start();
        _player = FindObjectOfType<PlayerController>().transform;
    }

    public override void Move(float speed)
    {
        _speed = speed;
       
        if(_startingDistance != 0)
        {
            rb.velocity = transform.up * _speed;
            float timeToStep = _startingDistance / _speed;
            Invoke(nameof(StepStop), timeToStep);
        }
        else
        {
            StepStop();
        }
    }

    protected override void Update()
    {
        base.Update();
        
        if(_canRotate && _lock)
            PointAtPlayer();
    }

    private void StepMov()
    {
        _canRotate = false;
        if (_stepIndex < _steps)
        {
            float timeToStep = _stepDistance / _speed;
            Invoke(nameof(StepStop), timeToStep);
        }

        rb.velocity = transform.up * _speed;
    }

    private void StepStop()
    {
        _canRotate = true;
        rb.velocity = Vector2.zero;
        _stepIndex++;
        Invoke(nameof(StepMov), _stepTime);
    }

    protected virtual void PointAtPlayer()
    {
        if (_rotationSpeed == 0 || (_stepIndex >= _steps && _lockOnLastStep))
        {
            transform.up = _player.position - transform.position;
        }
        else
        {
            Vector3 dir = _player.position - transform.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, _rotationSpeed * Time.deltaTime);
        }
    }
}
