using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class WaveGun : Gun
{
    [SerializeField]
    private bool _waveFollow;

    protected override void ManageShooting()
    {
        if (_gunBehavior[_gunBehaviorIndex]._fireRate != 0)
        {
            //If cooldown between shots has passed
            if (Time.time > _timeUntilShooting)
            {
                _timeUntilShooting = Time.time + _gunBehavior[_gunBehaviorIndex]._fireRate;

                //Shoot a shot
                WaveAttackBehavior wave = Instantiate(_gunBehavior[_gunBehaviorIndex]._bulletObject,
                    transform.position, Quaternion.identity,
                    _bulletPool).GetComponent<WaveAttackBehavior>();
                wave._speed = _gunBehavior[_gunBehaviorIndex]._bulletSpeed;
                wave._maxSize = _gunBehavior[_gunBehaviorIndex]._maxDistance;
                if(_waveFollow)
                {
                    wave._followTarget = transform;
                }
            }
        }
    }
}
