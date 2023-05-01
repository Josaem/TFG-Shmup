using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class GunContainer : MonoBehaviour
{    
    public bool _startActive = false;
    public float _waitUntilShooting;
    public float _speedOfAttack = 3;
    public float _maxDistance = 20;

    [SerializeField]
    protected GameObject _attackObject;

    private bool _active = false;

    protected PlayerController _playerController;
    protected Transform _bulletPool;

    protected virtual void Start()
    {
        _playerController = FindObjectOfType<PlayerController>();

        if (GetComponentInParent<CustomBulletPool>() != null)
            _bulletPool = GetComponentInParent<CustomBulletPool>()._newBP;

        if (_bulletPool == null)
        {
            if (GetComponentInParent<WaveObject>() == null)
            {
                _bulletPool = FindObjectOfType<BulletPool>().transform;
            }
            else
            {
                _bulletPool = GetComponentInParent<WaveObject>()._bulletPool.transform;
            }
        }

        if(_startActive)
        {
            if (_waitUntilShooting != 0)
                Invoke(nameof(Attack), _waitUntilShooting);
            else
                Attack();
        }
    }

    void Update()
    {
        if (_active)
        {
            ManageTargetting();
            ManageShooting();
        }
    }

    public void EnableGun()
    {
        if (_waitUntilShooting != 0)
            Invoke(nameof(Attack), _waitUntilShooting);
        else
            Attack();
    }

    protected virtual void Attack()
    {
        _active = true;
    }

    public virtual void DisableGun()
    {
        _active = false;
    }

    protected virtual void ManageShooting()
    {

    }

    protected virtual void ManageTargetting()
    {

    }
}
