using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class LaserGun : Gun
{
    private GameObject _currentLaser;
    private GameObject _previousLaser;

    protected override void ManageShooting()
    {
        if (_gunBehavior[_gunBehaviorIndex]._fireRate != 0 && _currentLaser != null)
        {
            _currentLaser.transform.position = transform.position;
            _currentLaser.transform.rotation = transform.rotation;
        }
    }

    protected override void StartGunBehavior()
    {
        base.StartGunBehavior();
        if(_gunBehavior[_gunBehaviorIndex]._fireRate != 0)
        {
            _currentLaser = Instantiate(_gunBehavior[_gunBehaviorIndex]._bulletObject,
                transform.position, transform.rotation,
                _bulletPool);
            if (_currentLaser.GetComponentInChildren<EnemyLaserBehavior>() != null)
            {
                _currentLaser.GetComponentInChildren<EnemyLaserBehavior>()._shootSpeed = _gunBehavior[_gunBehaviorIndex]._bulletSpeed;
            }
        }
    }

    public override void DisableShooting()
    {
        base.DisableShooting();

        Destroy(_currentLaser);
    }

    protected override void EndGunBehavior()
    {
        base.EndGunBehavior();

        _previousLaser = _currentLaser;
        Destroy(_previousLaser);
    }

    protected override void PointAtPlayer()
    {
        /*
        float rotTarget = Vector2.Angle(transform.up, _player.position - transform.position);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, rotTarget + 135), _gunBehavior[_gunBehaviorIndex]._bulletSpeed * Time.deltaTime);*/
    }
}
