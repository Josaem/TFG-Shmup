using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    protected GunBehavior[] _gunBehavior;

    private Vector3 _originalRot;
    protected int _gunBehaviorIndex = 0;
    protected Transform _player;
    private PlayerController _playerController;
    protected bool _shoot = false;
    protected float _timeUntilShooting;
    protected Transform _bulletPool;
    private float _rotTime = 0;

    [System.Serializable]
    protected class GunBehavior
    {
        #if UNITY_EDITOR
        [Help("If object is a laser _fireRate enables/disables it" +
            "\nIf object is a laser _bulletSpeed controls the ray speed" +
            "\nIf object is wave gun then point at player and rotative behavior are useless", UnityEditor.MessageType.None)]
        #endif
        public float _duration;
        public float _fireRate;
        public float _bulletSpeed;
        public float _maxDistance = 20;
        public bool _pointAtPlayer = false;
        public RotativeBehavior _rotativeBehavior;
        public GameObject _bulletObject;
    }

    [System.Serializable]
    protected class RotativeBehavior
    {
        public RotateStart _rotateStart = RotateStart.None;
        public bool _dontCenterAngleRotation = false;
        public bool _resetRotTimer;
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

    // Start is called before the first frame update
    void Start()
    {
        _originalRot = transform.localEulerAngles;
        _player = FindObjectOfType<PlayerController>().transform;
        _bulletPool = GetComponentInParent<WaveObject>()._bulletPool.transform;
        _playerController = _player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_shoot)
        {
            _rotTime += Time.deltaTime;
            ManageTargetting();
            ManageShooting();
        }
    }

    public void EnableShooting()
    {
        _gunBehaviorIndex = 0;
        _shoot = true;
        _rotTime = 0;

        StartGunBehavior();
    }

    public virtual void DisableShooting()
    {
        _shoot = false;
    }

    protected virtual void StartGunBehavior()
    {
        if(_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._resetRotTimer)
        {
            _rotTime = 0;
        }

        if (_gunBehavior[_gunBehaviorIndex]._duration != 0)
        {
            Invoke(nameof(EndGunBehavior), _gunBehavior[_gunBehaviorIndex]._duration);
        }
    }

    protected virtual void EndGunBehavior()
    {
        _gunBehaviorIndex++;
        if (_gunBehaviorIndex >= _gunBehavior.Length)
        {
            _gunBehaviorIndex = 0;
        }

        Invoke(nameof(StartGunBehavior), 0.05f);
    }

    protected virtual void ManageShooting()
    {
        if (_gunBehavior[_gunBehaviorIndex]._fireRate != 0)
        {
            //If cooldown between shots has passed
            if (Time.time > _timeUntilShooting)
            {
                _timeUntilShooting = Time.time + _gunBehavior[_gunBehaviorIndex]._fireRate;

                //Shoot a shot
                EnemyBulletBehavior bullet = Instantiate(_gunBehavior[_gunBehaviorIndex]._bulletObject,
                    transform.position, transform.rotation,
                    _bulletPool).GetComponent<EnemyBulletBehavior>();
                bullet._speed = _gunBehavior[_gunBehaviorIndex]._bulletSpeed;
                bullet._maxDistance = _gunBehavior[_gunBehaviorIndex]._maxDistance;
            }
        }
    }

    private void ManageTargetting()
    {
        if (_gunBehavior[_gunBehaviorIndex]._pointAtPlayer)
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
        else if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotateStart != RotateStart.None)
        {
            RotateWeapon();
        }
        else
        {
            ResetRotation();
        }
    }

    private void ResetRotation()
    {
        transform.localEulerAngles = _originalRot;
    }

    private void RotateWeapon()
    {
        if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle == 0)
        {
            if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotateStart == RotateStart.Left)
            {
                transform.Rotate(0, 0, _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(0, 0, -_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            if(_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._dontCenterAngleRotation)
            {
                if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotateStart == RotateStart.Right)
                {
                    if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, -Mathf.PingPong(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                            _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle));
                    }
                    else
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, -Mathf.Repeat(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                            _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle));
                    }
                }
                else
                {
                    if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.PingPong(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle));
                    }
                    else
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.Repeat(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                            _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle));
                    }
                }

            }
            else
            {
                if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotateStart == RotateStart.Right)
                {
                    if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, -(Mathf.PingPong(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle)
                        - _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle / 2));
                    }
                    else
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, -(Mathf.Repeat(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle)
                        - _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle / 2));
                    }                                
                }
                else
                {                 
                    if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotType == RotType.Pingpong)
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.PingPong(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle)
                        - _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle / 2);
                    }
                    else
                    {
                        transform.localEulerAngles = _originalRot + new Vector3(0, 0, Mathf.Repeat(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                        _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle)
                        - _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle / 2);
                    }
                }
            }
        }
    }

    protected virtual void PointAtPlayer()
    {
        transform.up = _player.position - transform.position;
    }
}
