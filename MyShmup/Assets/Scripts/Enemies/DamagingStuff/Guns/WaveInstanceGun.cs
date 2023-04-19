
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveInstanceGun : GunContainer
{
    [SerializeField]
    private float _timeBetweenWaves;
    
    [SerializeField]
    private GameObject _waveObject;

    private float _waveTime = 0;

    protected override void ManageShooting()
    {
        //If cooldown between shots has passed
        if (Time.time > _waveTime)
        {
            _waveTime = Time.time + _timeBetweenWaves;

            //Shoot a shot
            WaveAttackBehavior wave = Instantiate(_waveObject,
                transform.position, Quaternion.identity, _bulletPool).GetComponent<WaveAttackBehavior>();
            wave._speed = _speedOfAttack;
            wave._maxSize = _maxDistance*2;
        }
    }
}
