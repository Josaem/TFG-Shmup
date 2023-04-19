using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileGun : RotativeGuns
{
    public float _fireRate;

    protected float _timeUntilShooting = 0;

    protected override void ManageShooting()
    {
        if (_fireRate > 0)
        {
            //If cooldown between shots has passed
            if (Time.time > _timeUntilShooting)
            {
                _timeUntilShooting = Time.time + _fireRate;

                //Shoot a shot
                ProyectileBehavior bullet = Instantiate(_attackObject,
                    transform.position, transform.rotation,
                    _bulletPool).GetComponent<ProyectileBehavior>();
                bullet._speed = _attackSpeed;
                bullet._maxDistance = _maxDistance;
            }
        }
    }
}
