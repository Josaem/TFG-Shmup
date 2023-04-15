using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShootingEnemy : Enemy
{
    [SerializeField]
    private AttackType[] _attackPattern;
    [SerializeField]
    private bool _shootOnSpawn;
    [SerializeField]
    protected bool _stopShootOnDead = true;
    [SerializeField]
    private bool _changeAttackOnEntry;

    private int _attackIndex = 0;

    //Which weapons attack and for how long
    [System.Serializable]
    private class AttackType
    {
        public Weapon[] _weapons;
        public float _duration;
        public float _delayUntilNextAttack;
    }

    public override void Kill()
    {
        base.Kill();
        if(_stopShootOnDead)
            StopAttackingPermanently();
    }

    protected override void Die()
    {
        StopAttackingPermanently();
        base.Die();
    }

    protected override void Spawn()
    {
        base.Spawn();
        if(_shootOnSpawn)
        {
            StartAttacking();
        }
        else
        {
            _changeAttackOnEntry = false;
        }
    }

    public override void StartAction()
    {
        if (!_shootOnSpawn)
        {
            StartAttacking();
        }

        if (_shootOnSpawn && _changeAttackOnEntry)
        {
            _changeAttackOnEntry = false;
            StopAttacking();
        }
    }

    public void StartAttacking()
    {
        if(_attackPattern.Length != 0 && !(_movementState == EnemyMovementState.Dying && _stopShootOnDead))
        {
            foreach (Weapon weapon in _attackPattern[_attackIndex]._weapons)
            {
                weapon.EnableWeapon();
            }
                       
            if (!_changeAttackOnEntry && _attackPattern[_attackIndex]._duration != 0)
            {
                Invoke(nameof(StopAttacking), _attackPattern[_attackIndex]._duration);
            }
        }
    }

    private void StopAttacking()
    {
        if(_attackPattern.Length != 0)
        {
            foreach (Weapon weapon in _attackPattern[_attackIndex]._weapons)
            {
                weapon.DisableWeapon();
            }

            Invoke(nameof(StartAttacking), _attackPattern[_attackIndex]._delayUntilNextAttack);

            //Next attack
            _attackIndex++;

            //if no more attacks go to first one
            if (_attackIndex >= _attackPattern.Length)
            {
                _attackIndex = 0;
            }
        }
    }

    protected void StopAttackingPermanently()
    {
        if (_attackPattern.Length != 0)
        {
            foreach (Weapon weapon in _attackPattern[_attackIndex]._weapons)
            {
                weapon.DisableWeapon();
            }
        }
    }
}
