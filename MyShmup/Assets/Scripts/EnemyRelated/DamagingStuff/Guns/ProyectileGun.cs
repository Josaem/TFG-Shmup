using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileGun : RotativeGuns
{
    public float _fireRate;

    protected float _timeUntilShooting = 0;

    protected override void ManageShooting()
    {
        if (_fireRate > 0 && _active)
        {
            //If cooldown between shots has passed
            if (Time.time > _timeUntilShooting)
            {
                _timeUntilShooting = Time.time + _fireRate;
                
                //Shoot a shot
                ProyectileBehavior bullet = Instantiate(_attackObject,
                    transform.position, transform.rotation,
                    _bulletPool).GetComponent<ProyectileBehavior>();
                bullet.Move(_speedOfAttack);
                bullet.SetDeath(_maxDistance);
            }
        }
    }

    protected override void Attack()
    {
        base.Attack();
        _timeUntilShooting = 0;
    }
}
