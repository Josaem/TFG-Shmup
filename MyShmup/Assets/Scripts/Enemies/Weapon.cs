using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private float _delayUntilShooting = 0;
    [SerializeField]
    private WeaponBehavior[] _weaponBehaviors;

    private Transform _weaponPivot;
    private Vector3 _originalTransform;
    private bool _isEnabled = false;
    private Gun[] _guns;
    private int _weaponBehaviorIndex = 0;
    private Transform _player;

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
        public RotateStart _rotateStart;
        public float _rotationAngle;
        public float _rotationSpeed;
    }

    private enum RotateStart
    {
        None,
        Left,
        Right,
    };

    // Start is called before the first frame update
    void Start()
    {
        _originalTransform = transform.localEulerAngles;
        _weaponPivot = transform.GetChild(0);
        _guns = GetComponentsInChildren<Gun>();
        _player = FindObjectOfType<PlayerController>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isEnabled)
        {
            ManageTargetting();
        }
    }

    public void EnableWeapon()
    {
        _isEnabled = true;
        Invoke(nameof(StartWeapon), _delayUntilShooting);
    }

    private void StartWeapon()
    {
        Debug.Log("Starting Weapon");

        _weaponBehaviorIndex = 0;

        StartWeaponBehavior();
    }

    private void StartWeaponBehavior()
    {
        if(_isEnabled)
        {
            Debug.Log("Starting Gun, attack: " + _weaponBehaviorIndex);

            foreach(Gun gun in _guns)
            {
                gun.EnableShooting();
            }

            if(_weaponBehaviors[_weaponBehaviorIndex]._duration != 0)
            {
                Invoke(nameof(EndWeaponBehavior), _weaponBehaviors[_weaponBehaviorIndex]._duration);
            }
        }
    }

    private void EndWeaponBehavior()
    {
        Debug.Log("Ending Gun");

        foreach (Gun gun in _guns)
        {
            gun.DisableShooting();
        }

        _weaponBehaviorIndex++;
        if(_weaponBehaviorIndex >= _weaponBehaviors.Length)
        {
            _weaponBehaviorIndex = 0;
            Invoke(nameof(StartWeaponBehavior), _weaponBehaviors[_weaponBehaviorIndex]._delayUntilNextAttack);
        }
        else
        {
            Invoke(nameof(StartWeaponBehavior), _weaponBehaviors[_weaponBehaviorIndex - 1]._delayUntilNextAttack);
        }
    }

    public void DisableWeapon()
    {
        _isEnabled = false;

        Debug.Log("Ending Weapon");

        foreach (Gun gun in _guns)
        {
            gun.DisableShooting();
        }
    }

    private void ManageTargetting()
    {
        if (_weaponBehaviors[_weaponBehaviorIndex]._pointAtPlayer)
        {
            transform.up = _player.position - transform.position;
        }
        else
        {
            transform.localEulerAngles = _originalTransform;
        }

        if (_weaponBehaviors[_weaponBehaviorIndex]._rotativeBehavior._rotateStart != RotateStart.None)
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
        if(_weaponBehaviors[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle == 0)
        {
            if(_weaponBehaviors[_weaponBehaviorIndex]._rotativeBehavior._rotateStart == RotateStart.Left)
            {
                _weaponPivot.Rotate(0, 0, _weaponBehaviors[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed * Time.deltaTime);
            }
            else
            {
                _weaponPivot.Rotate(0, 0, -_weaponBehaviors[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            _weaponPivot.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(Time.time * _weaponBehaviors[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed,
                    _weaponBehaviors[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle)
                    - _weaponBehaviors[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle/2);
        }

        
    }
}
