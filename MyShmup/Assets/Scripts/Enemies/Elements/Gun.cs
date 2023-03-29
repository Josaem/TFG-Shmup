using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField]
    protected GunBehavior[] _gunBehavior;

    private Vector3 _originalTransform;
    protected int _gunBehaviorIndex = 0;
    protected Transform _player;
    private PlayerController _playerController;
    protected bool _shoot = false;
    private float _timeUntilShooting;
    protected Transform _bulletPool;
    private float _rotTime = 0;

    [System.Serializable]
    protected class GunBehavior
    {
        public float _duration;
        public float _fireRate;
        public float _bulletSpeed;
        public bool _pointAtPlayer = false;
        public RotativeBehavior _rotativeBehavior;
        public GameObject _bulletObject;
    }

    [System.Serializable]
    protected class RotativeBehavior
    {
        public RotateStart _rotateStart = RotateStart.None;
        public float _rotationAngle;
        public float _rotationSpeed;
    }

    protected enum RotateStart
    {
        None,
        Left,
        Right,
    };

    // Start is called before the first frame update
    void Start()
    {
        _originalTransform = transform.localEulerAngles;
        _player = FindObjectOfType<PlayerController>().transform;
        _bulletPool = GameObject.FindWithTag("BulletPool").transform;
        _playerController = _player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_shoot)
        {
            _rotTime += Time.deltaTime;
            ManageShooting();
            ManageTargetting();
        }
    }

    public void EnableShooting()
    {
        _gunBehaviorIndex = 0;
        _shoot = true;
        _rotTime = 0;

        StartGunBehavior();
    }

    public void DisableShooting()
    {
        _shoot = false;
    }

    protected virtual void StartGunBehavior()
    {
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
                GameObject bullet = Instantiate(_gunBehavior[_gunBehaviorIndex]._bulletObject,
                    transform.position, transform.rotation,
                    _bulletPool);
                bullet.GetComponent<EnemyBulletBehavior>()._speed = _gunBehavior[_gunBehaviorIndex]._bulletSpeed;
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
        transform.localEulerAngles = _originalTransform;
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
            if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotateStart == RotateStart.Left)
            {
                transform.localEulerAngles = new Vector3(0, 0, -(Mathf.PingPong(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                    _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle)
                    - _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle / 2));
            }
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, Mathf.PingPong(_rotTime * _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationSpeed,
                    _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle)
                    - _gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotationAngle / 2);
            }
        }
    }

    protected virtual void PointAtPlayer()
    {
        transform.up = _player.position - transform.position;
    }
}
