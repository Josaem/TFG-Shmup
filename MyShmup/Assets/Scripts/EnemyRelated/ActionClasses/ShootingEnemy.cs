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
    [Help("If change on attack behavior will loop ignoring the first behavior\nThe duration of the pattern will delay when it changes behavior", UnityEditor.MessageType.None)]
    #endif
    [SerializeField]
    private bool _changeAttackOnEntry;

    private float _timeToChangeAttack;

    private bool _skipForChangeOnEntry = true;
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
        if (_stopShootOnDead)
            StopAttacking();
        base.Kill();
    }

    protected override void Die()
    {
        StopAttacking();
        base.Die();
    }

    protected override void Start()
    {
        base.Start();

        _timeToChangeAttack = _attackPattern[_attackIndex]._duration;
        if (!_changeAttackOnEntry)
            _skipForChangeOnEntry = false;
    }

    protected override void Spawn()
    {
        base.Spawn();

        if(_shootOnSpawn)
        {
            StartAttacking();
        }
    }

    public override void StartAction()
    {
        if (!_shootOnSpawn)
        {
            if (_delayUntilFirstAction != 0)
                Invoke(nameof(StartAttacking), _delayUntilFirstAction);
            else
                StartAttacking();
        }
        else if (_changeAttackOnEntry)
        {
            if (_timeToChangeAttack != 0)
                Invoke(nameof(NextAttack), _timeToChangeAttack);
            else
                NextAttack();
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

            if (!_skipForChangeOnEntry && _attackPattern[_attackIndex]._duration != 0)
            {
                Invoke(nameof(NextAttack), _attackPattern[_attackIndex]._duration);
            }
        }
    }

    private void NextAttack()
    {
        if (_attackPattern.Length != 0)
        {
            if (_changeAttackOnEntry)
                _skipForChangeOnEntry = false;

            foreach (Weapon weapon in _attackPattern[_attackIndex]._weapons)
            {
                bool willBeUsedNext = false;

                if (_attackPattern[_attackIndex]._delayUntilNextAttack == 0)
                {
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
                }
                else
                    willBeUsedNext = false;

                if(!willBeUsedNext)
                    weapon.DisableWeapon();
            }

            float _currentDelay = _attackPattern[_attackIndex]._delayUntilNextAttack;

            _attackIndex++;

            //if no more attacks go to first one if not changeattack
            if (_attackIndex >= _attackPattern.Length)
            {
                if (_changeAttackOnEntry && _attackPattern.Length > 1)
                {
                    _attackIndex = 1;
                }
                else _attackIndex = 0;
            }

            if (_currentDelay != 0)
            {

                Invoke(nameof(StartAttacking), _currentDelay);
            }
            else
            {
                StartAttacking();
            }  
        }
    }

    protected void StopAttacking()
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
