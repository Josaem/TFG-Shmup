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

    private Vector3 _originalRot;
    private Transform _weaponPivot;
    private Vector3 _originalTransform;
    private bool _isEnabled = false;
    private Gun[] _guns;
    private GunContainer[] _gunsNew;
    private int _weaponBehaviorIndex = 0;
    private Transform _player;
    private float _rotTime = 0;

    //Duration of gun activation, where the weapon shoots
    [System.Serializable]
    private class WeaponBehavior
    {
        public float _duration;
        public float _delayUntilNextAttack;
        public bool _pointAtPlayer;
        public RotativeBehavior _rotativeBehavior;
    }

    [System.Serializable]
    private class RotativeBehavior
    {
        public RotateStart _rotateStart = RotateStart.None;
        public bool _dontCenterAngleRotation = false;
        public bool _resetRotTimer;
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

    // Start is called before the first frame update
    void Start()
    {
        _originalTransform = transform.localEulerAngles;
        _weaponPivot = transform.GetChild(0);
        _guns = GetComponentsInChildren<Gun>();
        _gunsNew = GetComponentsInChildren<GunContainer>();
        _player = FindObjectOfType<PlayerController>().transform;
        _originalRot = _weaponPivot.transform.localEulerAngles;

        if(_startEnabled)
        {
            EnableWeapon();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_isEnabled)
        {
            _rotTime += Time.deltaTime;
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
        _weaponBehaviorIndex = 0;
        _rotTime = 0;
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

        _weaponBehaviorIndex++;
        if(_isEnabled)
        {
            if (_weaponBehaviorIndex >= _weaponBehavior.Length)
            {
                _weaponBehaviorIndex = 0;
                Invoke(nameof(StartWeaponBehavior), _weaponBehavior[_weaponBehaviorIndex]._delayUntilNextAttack);
            }
            else
            {
                Invoke(nameof(StartWeaponBehavior), _weaponBehavior[_weaponBehaviorIndex - 1]._delayUntilNextAttack);
            }
        }
    }

    private void ManageTargetting()
    {
        if (_weaponBehavior[_weaponBehaviorIndex]._pointAtPlayer)
        {
            transform.up = _player.position - transform.position;
        }
        else
        {
            transform.localEulerAngles = _originalTransform;
        }

        if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotateStart != RotateStart.None)
        {
            RotateWeapon();
        }
        else
        {
            _weaponPivot.localEulerAngles = new Vector3(0,0,0);
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
}
