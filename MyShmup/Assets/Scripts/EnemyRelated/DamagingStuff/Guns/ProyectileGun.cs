using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProyectileGun : RotativeGuns
{
#if UNITY_EDITOR
    [Help("If maxDistance is 0 the bullet doesn't die by timer", UnityEditor.MessageType.None)]
#endif
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
                if(_maxDistance > 0)
                {
                    bullet.SetDeath(_maxDistance);
                }
            }
        }
    }

    protected override void Attack()
    {
        base.Attack();
        _timeUntilShooting = 0;
    }
}
