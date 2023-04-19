using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserInstanceGun : RotativeGuns
{
    public float _guideTime = 0.5f;
    private GameObject _currentLaser;

    protected override void ManageShooting()
    {
        _currentLaser.transform.SetPositionAndRotation(transform.position, transform.rotation);
    }

    protected override void Attack()
    {
        base.Attack();
        _currentLaser = Instantiate(_attackObject,
            transform.position, transform.rotation,
            _bulletPool);

        if(TryGetComponent<LaserBehavior>(out LaserBehavior laser))
        {
            laser._laserSpeed = _speedOfAttack;
            laser._maxDistance = _maxDistance;
            laser._timeToSpawn = _guideTime;
        }
    }

    public override void DisableGun()
    {
        base.DisableGun();

        _currentLaser.GetComponentInChildren<LaserBehavior>().Kill();
    }
}
