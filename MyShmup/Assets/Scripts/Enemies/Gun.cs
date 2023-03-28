using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Gun : MonoBehaviour
{
    [SerializeField]
    private GunBehavior[] _gunBehavior;

    private Vector3 _originalTransform;
    private int _gunBehaviorIndex = 0;
    private Transform _player;
    private PlayerController _playerController;
    private bool _shoot = false;
    private float _timeUntilShooting;
    private Transform _playerBulletPool;
    private float _rotTime = 0;

    [System.Serializable]
    private class GunBehavior
    {
        public float _duration;
        public float _fireRate;
        public float _bulletSpeed;
        public bool _pointAtPlayer = false;
        public RotativeBehavior _rotativeBehavior;
        public GameObject _bulletObject;
        public GameObject _bulletMovement; //TODO change this to a "MovementType" script
    }

    [System.Serializable]
    private class RotativeBehavior
    {
        public RotateStart _rotateStart = RotateStart.None;
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
        _player = FindObjectOfType<PlayerController>().transform;
        _playerBulletPool = GameObject.FindWithTag("BulletPool").transform;
        _playerController = _player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_shoot && !_playerController._dead)
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

    private void StartGunBehavior()
    {
        if (_gunBehavior[_gunBehaviorIndex]._duration != 0)
        {
            Invoke(nameof(EndGunBehavior), _gunBehavior[_gunBehaviorIndex]._duration);
        }
    }

    private void EndGunBehavior()
    {
        _gunBehaviorIndex++;
        if (_gunBehaviorIndex >= _gunBehavior.Length)
        {
            _gunBehaviorIndex = 0;
        }

        Invoke(nameof(StartGunBehavior), 0.05f);
    }

    private void ManageShooting()
    {
        //If cooldown between shots has passed
        if (Time.time > _timeUntilShooting)
        {
            _timeUntilShooting = Time.time + _gunBehavior[_gunBehaviorIndex]._fireRate;

            //Shoot a shot
            GameObject bullet = Instantiate(_gunBehavior[_gunBehaviorIndex]._bulletObject,
                transform.position, transform.rotation,
                _playerBulletPool);
            bullet.GetComponent<EnemyBulletBehavior>()._speed = _gunBehavior[_gunBehaviorIndex]._bulletSpeed;
        }
    }

    private void ManageTargetting()
    {
        if (_gunBehavior[_gunBehaviorIndex]._pointAtPlayer)
        {
            transform.up = _player.position - transform.position;
        }
        else if (_gunBehavior[_gunBehaviorIndex]._rotativeBehavior._rotateStart != RotateStart.None)
        {
            RotateWeapon();
        }
        else
        {
            transform.localEulerAngles = _originalTransform;
        }
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

}
