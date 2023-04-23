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
    #if UNITY_EDITOR
    [Help("If change on attack behavior will loop ignoring the first behavior", UnityEditor.MessageType.None)]
    #endif
    [SerializeField]
    private bool _changeAttackOnEntry;

    private int _attackIndex = 0;
    private bool _changedAttackOnEntry;

    //Which weapons attack and for how long
    [System.Serializable]
    private class AttackType
    {
        public Weapon[] _weapons;
        public float _duration;
        public float _delayUntilNextAttack;
    }

    protected override void Start()
    {
        base.Start();

        if (_changeAttackOnEntry)
            _changedAttackOnEntry = true;
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
            _changedAttackOnEntry = false;
        }
    }

    public override void StartAction()
    {
        if (!_shootOnSpawn)
        {
            StartAttacking();
        }

        if (_shootOnSpawn && _changedAttackOnEntry)
        {
            _changedAttackOnEntry = false;
            StopAttacking();
        }
    }

    public void StartAttacking()
    {
        if(_attackPattern.Length != 0 && !(_movementState == EnemyMovementState.Dying && _stopShootOnDead))
        {
            foreach (Weapon weapon in _attackPattern[_attackIndex]._weapons)
            {
                if(!weapon._isEnabled)
                    weapon.EnableWeapon();
            }
                       
            if (!_changedAttackOnEntry && _attackPattern[_attackIndex]._duration != 0)
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
                bool willBeUsedNext = false;

                //Check if next weapon to use is already active
                if (_attackIndex + 1 >= _attackPattern.Length)
                {
                    foreach (Weapon weaponToEnable in _attackPattern[0]._weapons)
                        if (weaponToEnable == weapon)
                            willBeUsedNext = true;
                }
                else
                {
                    foreach (Weapon weaponToEnable in _attackPattern[_attackIndex + 1]._weapons)
                        if (weaponToEnable == weapon)
                            willBeUsedNext = true;
                }

                if(!willBeUsedNext)
                    weapon.DisableWeapon();
            }

            Invoke(nameof(StartAttacking), _attackPattern[_attackIndex]._delayUntilNextAttack);

            //Next attack
            _attackIndex++;

            //if no more attacks go to first one if not changeattack
            if (_attackIndex >= _attackPattern.Length)
            {
                if (_changeAttackOnEntry) _attackIndex = 1;
                else _attackIndex = 0;
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
