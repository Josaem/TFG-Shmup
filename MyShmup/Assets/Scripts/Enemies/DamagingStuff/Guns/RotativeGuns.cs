using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotativeGuns : GunContainer
{
    public PointTowardsPlayer _pointAtPlayer = PointTowardsPlayer.None;
    [SerializeField]
    protected RotativeBehavior _rotativeBehavior;

    private Vector3 _originalRot;
    private float _rotTime = 0;

    [System.Serializable]
    protected class RotativeBehavior
    {
        public RotateStart _rotateStart = RotateStart.None;
        public bool _dontCenterAngleRotation;
        public bool _resetRotTimer;
        public bool _resetRot;
        public RotType _rotType;
        public float _rotationAngle;
        public float _rotationSpeed;
    }

    protected enum RotateStart
    {
        None,
        Left,
        Right,
    };

    protected enum RotType
    {
        Pingpong,
        Repeat
    };

    public enum PointTowardsPlayer
    {
        None,
        Instant,
        Slow
    };

    protected override void Start()
    {
        base.Start();
        _originalRot = transform.localEulerAngles;
    }

    protected override void Attack()
    {
        base.Attack();

        if (_rotativeBehavior._resetRotTimer)
            _rotTime = 0;

        if (_rotativeBehavior._resetRot)
            ResetRotation();
    }

    protected override void ManageTargetting()
    {
        _rotTime += Time.deltaTime;

        if (_pointAtPlayer != PointTowardsPlayer.None)
        {
            if (!_playerController._dead)
            {
                PointAtPlayer();
            }
            else
            {
                ResetRotation();
            }
        }
        else if (_rotativeBehavior._rotateStart != RotateStart.None)
        {
            RotateWeapon();
        }
    }

    private void ResetRotation()
    {
        transform.localEulerAngles = _originalRot;
    }

    private void RotateWeapon()
    {
        if (_rotativeBehavior._rotationAngle == 0)
        {
            if (_rotativeBehavior._rotateStart == RotateStart.Left)
            {
                transform.Rotate(0, 0, _rotativeBehavior._rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(0, 0, -_rotativeBehavior._rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (_rotativeBehavior._dontCenterAngleRotation)
            {
                if (_rotativeBehavior._rotateStart == RotateStart.Right)
                {
                    if (_rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, -Mathf.PingPong(_rotTime * _rotativeBehavior._rotationSpeed,
                            _rotativeBehavior._rotationAngle));
                    }
                    else
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, -Mathf.Repeat(_rotTime * _rotativeBehavior._rotationSpeed,
                            _rotativeBehavior._rotationAngle));
                    }
                }
                else
                {
                    if (_rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.PingPong(_rotTime * _rotativeBehavior._rotationSpeed,
                        _rotativeBehavior._rotationAngle));
                    }
                    else
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.Repeat(_rotTime * _rotativeBehavior._rotationSpeed,
                            _rotativeBehavior._rotationAngle));
                    }
                }

            }
            else
            {
                if (_rotativeBehavior._rotateStart == RotateStart.Right)
                {
                    if (_rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, -(Mathf.PingPong(_rotTime * _rotativeBehavior._rotationSpeed,
                        _rotativeBehavior._rotationAngle)
                        - _rotativeBehavior._rotationAngle / 2));
                    }
                    else
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, -(Mathf.Repeat(_rotTime * _rotativeBehavior._rotationSpeed,
                        _rotativeBehavior._rotationAngle)
                        - _rotativeBehavior._rotationAngle / 2));
                    }
                }
                else
                {
                    if (_rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.PingPong(_rotTime * _rotativeBehavior._rotationSpeed,
                        _rotativeBehavior._rotationAngle)
                        - _rotativeBehavior._rotationAngle / 2);
                    }
                    else
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.Repeat(_rotTime * _rotativeBehavior._rotationSpeed,
                        _rotativeBehavior._rotationAngle)
                        - _rotativeBehavior._rotationAngle / 2);
                    }
                }
            }
        }
    }

    protected virtual void PointAtPlayer()
    {
        if (_pointAtPlayer == PointTowardsPlayer.Instant)
        {
            transform.up = _playerController.transform.position - transform.position;
        }
        else
        {

        }
    }
}