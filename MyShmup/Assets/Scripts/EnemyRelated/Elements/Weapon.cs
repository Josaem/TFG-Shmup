using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    private bool _startEnabled;
    [SerializeField]
    private float _delayUntilShooting = 0;
    [SerializeField]
    private WeaponBehavior[] _weaponBehavior;

    [HideInInspector]
    public bool _isEnabled = false;
    private GunContainer[] _guns;
    private int _weaponBehaviorIndex = 0;

    private PlayerController _playerController;

    private Transform _weaponPivot;
    private Vector3 _originalTransform;
    private Vector3 _originalRot;
    private float _rotTime = 0;
    private float _rotOffset = 0;
    private bool _hasLockedPlayer;
    private Vector3 _singleLockPos;
    private IEnumerator _initWeapon;
    private IEnumerator _startWeapon;
    private IEnumerator _endWeapon;

    //Duration of gun activation, where the weapon shoots
    [System.Serializable]
    private class WeaponBehavior
    {
        public float _duration;
        public float _delayUntilNextAttack;
        public PointTowardsPlayer _pointAtPlayer = PointTowardsPlayer.None;
        public float _pointAtPlayerSpeed = 10;
        public RotativeBehavior _rotativeBehavior;
    }

    [System.Serializable]
    private class RotativeBehavior
    {
        #if UNITY_EDITOR
        [Help("_resetRot refers to the pointAtPlayer rotation, while _resetRotTimer refers to the rotate behavior", UnityEditor.MessageType.None)]
        #endif
        public RotateStart _rotateStart = RotateStart.None;
        public bool _dontCenterAngleRotation;
        public bool _resetRotTimer;
        public bool _resetRot;
        public bool _startRotationInCenter;
        public RotType _rotType;
        public float _rotationAngle;
        public float _rotationSpeed;
        public AnimationCurve _rotCurve;
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
        Repeat,
        Curve
    };

    public enum PointTowardsPlayer
    {
        None,
        Instant,
        Slow,
        Single
    };

    // Start is called before the first frame update
    void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();

        _originalTransform = transform.localEulerAngles;
        _weaponPivot = transform.GetChild(0);
        _originalRot = _weaponPivot.transform.localEulerAngles;

        if (_startEnabled)
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
        _guns = GetComponentsInChildren<GunContainer>();

        _initWeapon = InitWeapon(_delayUntilShooting);
        StartCoroutine(_initWeapon);
    }

    public void DisableWeapon()
    {
        _isEnabled = false;
        if(_initWeapon != null)
            StopCoroutine(_initWeapon);
        if (_endWeapon != null)
            StopCoroutine(_endWeapon);
        if (_startWeapon != null)
            StopCoroutine(_startWeapon);

        EndWeaponBehavior();
    }

    private IEnumerator InitWeapon(float time)
    {
        yield return new WaitForSeconds(time);

        InitializeWeapon();
    }

    private void InitializeWeapon()
    {
        ResetRotValues();

        _startWeapon = StartWeapon(_weaponBehavior[_weaponBehaviorIndex]._duration);
        StartCoroutine(_startWeapon);
    }

    private void ResetRotValues()
    {
        if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._resetRotTimer)
        {
            ResetPivotRotation();
            _rotTime = 0;
        }

        if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._startRotationInCenter)
            _rotOffset = 0.5f;
        else _rotOffset = 0;

        if (_weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._resetRot)
            ResetWeaponRotation();


        if (_weaponBehavior[_weaponBehaviorIndex]._pointAtPlayer != PointTowardsPlayer.Slow) ResetWeaponRotation();
        ManageTargetting();
    }

    private IEnumerator StartWeapon(float time)
    {
        ResetRotValues();
        StartWeaponBehavior();

        if (time > 0)
        {
            yield return new WaitForSeconds(time);
        }
        else
        {
            yield return null;
        }

        if (_weaponBehavior[_weaponBehaviorIndex]._duration != 0)
        {
            _endWeapon = EndWeapon(_weaponBehavior[_weaponBehaviorIndex]._delayUntilNextAttack);
            StartCoroutine(_endWeapon);
        }
    }

    private void StartWeaponBehavior()
    {
        _hasLockedPlayer = false;
        if(_isEnabled)
        {
            ManageTargetting();
            foreach (GunContainer gun in _guns)
            {
                gun.EnableGun();
            }
        }
    }

    private IEnumerator EndWeapon(float time)
    {
        EndWeaponBehavior();

        if (time > 0)
        {
            yield return new WaitForSeconds(time);
        }
        else
        {
            yield return null;
        }

        if (_isEnabled && _weaponBehavior[_weaponBehaviorIndex]._duration != 0)
        {
            if (_weaponBehaviorIndex + 1 >= _weaponBehavior.Length)
            {
                _weaponBehaviorIndex = 0;
                _startWeapon = StartWeapon(_weaponBehavior[_weaponBehaviorIndex]._duration);
                StartCoroutine(_startWeapon);
            }
            else
            {
                _startWeapon = StartWeapon(_weaponBehavior[++_weaponBehaviorIndex]._duration);
                StartCoroutine(_startWeapon);
            }
        }
    }

    private void EndWeaponBehavior()
    {
        if(_guns != null)
            foreach (GunContainer gun in _guns)
            {
                gun.DisableGun();
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
            float rotSpeed = _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationSpeed;
            float rotAngle = _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotationAngle;
            bool dontCenterAngleRot = _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._dontCenterAngleRotation;
            RotateStart rotateStart = _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotateStart;
            RotType rotType = _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotType;
            AnimationCurve rotCurve = _weaponBehavior[_weaponBehaviorIndex]._rotativeBehavior._rotCurve;

            if (rotType == RotType.Curve)
            {
                rotSpeed /= 100;
            }                

            if (dontCenterAngleRot)
            {
                if (rotateStart == RotateStart.Right)
                {
                    if (rotType == RotType.Pingpong)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, -Mathf.PingPong(_rotTime * rotSpeed, rotAngle));
                    }
                    else if(rotType == RotType.Repeat)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, -Mathf.Repeat(_rotTime * rotSpeed, rotAngle));
                    }
                    else
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0,
                            -(Mathf.Lerp(_originalRot.z - rotAngle / 2, _originalRot.z + rotAngle / 2, rotCurve.Evaluate(_rotTime * rotSpeed))
                                    + rotAngle / 2));                        
                    }
                }
                else
                {
                    if (rotType == RotType.Pingpong)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.PingPong(_rotTime * rotSpeed, rotAngle));
                    }
                    else if (rotType == RotType.Repeat)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.Repeat(_rotTime * rotSpeed, rotAngle));
                    }
                    else
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0,
                            Mathf.Lerp(_originalRot.z - rotAngle / 2, _originalRot.z + rotAngle / 2, rotCurve.Evaluate(_rotTime * rotSpeed))
                                    + rotAngle / 2);
                    }
                }
            }
            else
            {
                if (rotateStart == RotateStart.Right)
                {
                    if (rotType == RotType.Pingpong)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, -(Mathf.PingPong(_rotTime * rotSpeed + _rotOffset, rotAngle) - rotAngle / 2));
                    }
                    else if (rotType == RotType.Repeat)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, -(Mathf.Repeat(_rotTime * rotSpeed + _rotOffset, rotAngle) - rotAngle / 2));
                    }
                    else
                    {                        
                        _weaponPivot.transform.localEulerAngles = new Vector3(0, 0,
                            -Mathf.Lerp(_originalRot.z - rotAngle / 2, _originalRot.z + rotAngle / 2, rotCurve.Evaluate(_rotTime * rotSpeed + _rotOffset)));
                    }
                }
                else
                {
                    if (rotType == RotType.Pingpong)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.PingPong(_rotTime * rotSpeed + _rotOffset, rotAngle) - rotAngle / 2);
                    }
                    else if (rotType == RotType.Repeat)
                    {
                        _weaponPivot.transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.Repeat(_rotTime * rotSpeed + _rotOffset, rotAngle) - rotAngle / 2);
                    }
                    else
                    {
                        _weaponPivot.transform.localEulerAngles = new Vector3(0, 0,
                            Mathf.Lerp(_originalRot.z - rotAngle / 2, _originalRot.z + rotAngle / 2, rotCurve.Evaluate(_rotTime * rotSpeed + _rotOffset)));
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
        else if (_weaponBehavior[_weaponBehaviorIndex]._pointAtPlayer == PointTowardsPlayer.Single)
        {
            if (!_hasLockedPlayer)
            {
                _singleLockPos = _playerController.transform.position;
                _hasLockedPlayer = true;
            }

            transform.up = _singleLockPos - transform.position;
        }
        else
        {
            Vector3 dir = _playerController.transform.position - transform.position;
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
            Quaternion q = Quaternion.AngleAxis(targetAngle, Vector3.forward);
            transform.rotation = Quaternion.Lerp(transform.rotation, q, _weaponBehavior[_weaponBehaviorIndex]._pointAtPlayerSpeed * Time.deltaTime);
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

    private void OnDestroy()
    {
        if (_initWeapon != null)
            StopCoroutine(_initWeapon);
        if (_endWeapon != null)
            StopCoroutine(_endWeapon);
        if (_startWeapon != null)
            StopCoroutine(_startWeapon);
    }
}
