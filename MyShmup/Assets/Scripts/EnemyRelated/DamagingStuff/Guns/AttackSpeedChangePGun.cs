using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedChangePGun : RotativeGuns
{
    public float _fireRate = 1;
    public float _finalSpeed = 10;
    public float _speedToLerp = 1;

    private float _lerpedSpeed;
    private float _lerpedTime;


    protected float _timeUntilShooting = 0;

    protected override void Start()
    {
        base.Start();
        _lerpedSpeed = _speedOfAttack;
    }

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
                bullet.Move(_lerpedSpeed);
                bullet.SetDeath(_maxDistance);
            }
        }

        _lerpedTime += Time.deltaTime * _speedToLerp;
        _lerpedSpeed = Mathf.Lerp(_speedOfAttack, _finalSpeed, _lerpedTime);
    }

    public override void DisableGun()
    {
        base.DisableGun();
        _timeUntilShooting = 0;
        _lerpedTime = 0;
        _lerpedSpeed = _speedOfAttack;
    }
}
