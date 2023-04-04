using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class WaveGun : Gun
{
    protected override void ManageShooting()
    {
        if (_gunBehavior[_gunBehaviorIndex]._fireRate != 0)
        {
            //If cooldown between shots has passed
            if (Time.time > _timeUntilShooting)
            {
                _timeUntilShooting = Time.time + _gunBehavior[_gunBehaviorIndex]._fireRate;

                //Shoot a shot
                GameObject bullet = Instantiate(_gunBehavior[_gunBehaviorIndex]._bulletObject,
                    transform.position, Quaternion.identity,
                    _bulletPool);
                bullet.GetComponent<WaveAttackBehavior>()._speed = _gunBehavior[_gunBehaviorIndex]._bulletSpeed;
            }
        }
    }
}
