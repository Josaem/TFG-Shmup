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

        LaserBehavior laser = _currentLaser.GetComponentInChildren<LaserBehavior>();

        if (laser != null)
        {
            laser.Spawn(_speedOfAttack, _maxDistance, _guideTime);
        }
    }

    public override void DisableGun()
    {
        base.DisableGun();

        if (GetComponentInChildren<LaserBehavior>() != null)
        {
            _currentLaser.GetComponentInChildren<LaserBehavior>().Kill();
        }
        else
        {
            Destroy(_currentLaser);
        }
    }
}
