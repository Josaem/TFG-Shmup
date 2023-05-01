using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpeedChangePGun : RotativeGuns
{
    public float _fireRate = 1;
    public float _finalSpeed = 10;
    public float _attackDuration = 1;
    [SerializeField]
    private AnimationCurve _speedCurve;

    private float _lerpedSpeed;
    private float _lerpedTime;


    protected float _timeUntilShooting = 0;

    protected override void Start()
    {
        base.Start();
        _lerpedSpeed = _speedOfAttack;
        _lerpedTime = 0;
    }

    protected override void Attack()
    {
        base.Attack();
        _lerpedSpeed = _speedOfAttack;
        _lerpedTime = 0;
    }

    protected override void ManageShooting()
    {
        _lerpedTime += Time.deltaTime;
        _lerpedSpeed = Mathf.Lerp(_speedOfAttack, _finalSpeed, _speedCurve.Evaluate(_lerpedTime/_attackDuration));

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
    }

    public override void DisableGun()
    {
        base.DisableGun();
        _timeUntilShooting = 0;
        _lerpedTime = 0;
        _lerpedSpeed = _speedOfAttack;
    }
}
