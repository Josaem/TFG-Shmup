using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private bool _startEnabled;
    [SerializeField]
    private float _delayUntilShooting = 0;
    [SerializeField]
    private WeaponBehavior[] _weaponBehavior;

    private bool _isEnabled = false;
    private Gun[] _guns;
    private GunContainer[] _gunsNew;
    private int _weaponBehaviorIndex = 0;

    private PlayerController _playerController;

    private Transform _weaponPivot;
    private Vector3 _originalTransform;
    private Vector3 _originalRot;
    private float _rotTime = 0;

    //Duration of gun activation, where the weapon shoots
    [System.Serializable]
    private class WeaponBehavior
    {
        public float _duration;
        public float _delayUntilNextAttack;
        public PointTowardsPlayer _pointAtPlayer = PointTowardsPlayer.None;
        public RotativeBehavior _rotativeBehavior;
    }

    [System.Serializable]
    private class RotativeBehavior
    {
        public RotateStart _rotateStart = RotateStart.None;
        public bool _dontCenterAngleRotation;
        public bool _resetRotTimer;
        public bool _resetRot;
        public RotType _rotType;
        public float _rotationAngle;
        public float _rotationSpeed;
    }

    private enum RotateStart
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

    // Start is called before the first frame update
    void Start()
    {
        _guns = GetComponentsInChildren<Gun>();
        _gunsNew = GetComponentsInChildren<GunContainer>();

        _playerController = FindObjectOfType<PlayerController>();

        _originalTransform = transform.localEulerAngles;
        _weaponPivot = transform.GetChild(0);
        _originalRot = _weaponPivot.transform.localEulerAngles;

        if(_startEnabled)
        {
            EnableWeapon();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_isEnabled && _weaponBehaviorIndex < _weaponBehavior.Length)
        {
            ManageTargetting();
        }
    }

    public void EnableWeapon()
    {
        _isEnabled = true;

        Invoke(nameof(InitializeWeapon), _delayUntilShooting);
    }

    public void DisableWeapon()
    {
        _isEnabled = false;

        EndWeaponBehavior();
    }

    private void InitializeWeapon()
    {
        if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._resetRotTimer)
            _rotTime = 0;

        if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._resetRot)
            ResetPivotRotation();

        ResetWeaponRotation();
        ManageTargetting();

        StartWeaponBehavior();
    }

    private void StartWeaponBehavior()
    {
        if(_isEnabled)
        {
            foreach(Gun gun in _guns)
            {
                if(gun != null)
                    gun.EnableShooting();
            }

            foreach (GunContainer gun in _gunsNew)
            {
                if (gun != null)
                    gun.EnableGun();
            }

            if (_weaponBehavior[_weaponBehaviorIndex]._duration != 0)
            {
                Invoke(nameof(EndWeaponBehavior), _weaponBehavior[_weaponBehaviorIndex]._duration);
            }
        }
    }

    private void EndWeaponBehavior()
    {
        foreach (Gun gun in _guns)
        {
            gun.DisableShooting();
        }

        foreach (GunContainer gun in _gunsNew)
        {
            if (gun != null)
                gun.DisableGun();
        }

        if(_isEnabled)
        {
            if (_weaponBehaviorIndex + 1 >= _weaponBehavior.Length)
            {
                _weaponBehaviorIndex = 0;
                Invoke(nameof(StartWeaponBehavior), _weaponBehavior[_weaponBehaviorIndex]._delayUntilNextAttack);
            }
            else
            {
                Invoke(nameof(StartWeaponBehavior), _weaponBehavior[_weaponBehaviorIndex++]._delayUntilNextAttack);
            }
        }
    }

    private void ManageTargetting()
    {
        _rotTime += Time.deltaTime;

        if (_weaponBehavior[_weaponBehaviorIndex]._pointAtPlayer != PointTowardsPlayer.None)
        {
            if (!_playerController._dead)
            {
                PointAtPlayer();
            }
            else
            {
                ResetWeaponRotation();
            }
        }

        if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotateStart != RotateStart.None)
        {
            RotateWeapon();
        }
    }

    private void RotateWeapon()
    {
        if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle == 0)
        {
            if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotateStart == RotateStart.Left)
            {
                _weaponPivot.Rotate(0, 0, _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed * Time.deltaTime);
            }
            else
            {
                _weaponPivot.Rotate(0, 0, -_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._dontCenterAngleRotation)
            {
                if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotateStart == RotateStart.Right)
                {
                    if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, -Mathf.PingPong(_rotTime * _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed,
                            _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle));
                    }
                    else
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, -Mathf.Repeat(_rotTime * _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed,
                            _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle));
                    }
                }
                else
                {
                    if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.PingPong(_rotTime * _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle));
                    }
                    else
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.Repeat(_rotTime * _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed,
                            _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle));
                    }
                }

            }
            else
            {
                if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotateStart == RotateStart.Right)
                {
                    if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, -(Mathf.PingPong(_rotTime * _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle)
                        - _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle / 2));
                    }
                    else
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, -(Mathf.Repeat(_rotTime * _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle)
                        - _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle / 2));
                    }
                }
                else
                {
                    if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.PingPong(_rotTime * _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle)
                        - _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle / 2);
                    }
                    else
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.Repeat(_rotTime * _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle)
                        - _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle / 2);
                    }
                }
            }
        }
    }

    protected virtual void PointAtPlayer()
    {
        if (_weaponBehavior[_weaponBehaviorIndex]._pointAtPlayer == PointTowardsPlayer.Instant)
        {
            transform.up = _playerController.transform.position - transform.position;
        }
        else
        {

        }
    }

    private void ResetWeaponRotation()
    {
        transform.localEulerAngles = _originalTransform;
    }

    private void ResetPivotRotation()
    {
        _weaponPivot.localEulerAngles = _originalRot;
    }
}
