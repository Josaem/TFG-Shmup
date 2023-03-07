using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : Enemy
{
    [SerializeField]
    private AttackType[] _attackPattern;

    private int _attackIndex = 0;

    [System.Serializable]
    private class AttackType
    {
        public Weapon[] _weapons;
        public float _duration;
        public float _delayUntilNextAttack;
    }

    public override void StartAttacking()
    {
        Debug.Log("Start Shooting attack: " + _attackIndex);
        foreach (Weapon weapon in _attackPattern[_attackIndex]._weapons)
        {
            weapon.EnableWeapon();
        }

        Invoke(nameof(StopAttacking), _attackPattern[_attackIndex]._duration);
    }

    private void StopAttacking()
    {
        Debug.Log("Stop Shooting");
        foreach (Weapon weapon in _attackPattern[_attackIndex]._weapons)
        {
            weapon.DisableWeapon();
        }

        Invoke(nameof(StartAttacking), _attackPattern[_attackIndex]._delayUntilNextAttack);

        //Next attack
        _attackIndex++;

        //if no more attacks go to first one
        if(_attackIndex >= _attackPattern.Length)
        {
            _attackIndex = 0;
        }
    }

    public override void Move()
    {
        /*
        TODO
        -behavior
        -path
        -waypoints
         */
    }
}
