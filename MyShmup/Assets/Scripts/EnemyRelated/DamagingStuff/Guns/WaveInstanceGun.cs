
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInstanceGun : GunContainer
{
    [SerializeField]
    private float _timeBetweenWaves;

    private float _waveTime = 0;

    protected override void ManageShooting()
    {
        //If cooldown between shots has passed
        if (Time.time > _waveTime && _active)
        {
            _waveTime = Time.time + _timeBetweenWaves;

            //Shoot a shot
            WaveAttackBehavior wave = Instantiate(_attackObject,
                transform.position, Quaternion.identity, _bulletPool).GetComponent<WaveAttackBehavior>();
            if(wave != null)
            {
                wave._speed = _speedOfAttack;
                wave._maxSize = _maxDistance * 2;
            }
        }
    }

    protected override void Attack()
    {
        base.Attack();
        _waveTime = 0;
    }
}
