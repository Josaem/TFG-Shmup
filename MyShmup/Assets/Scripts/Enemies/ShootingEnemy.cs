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

    private int _attackIndex = 0;

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
        if(_shootOnSpawn)
        {
            StartAttacking();
        }
    }

    public override void StartAction()
    {
        if (!_shootOnSpawn)
        {
            StartAttacking();
        }
    }

    public void StartAttacking()
    {
        foreach (Weapon weapon in _attackPattern[_attackIndex]._weapons)
        {
            weapon.EnableWeapon();
        }

        if(_attackPattern[_attackIndex]._duration != 0)
        {
            Invoke(nameof(StopAttacking), _attackPattern[_attackIndex]._duration);
        }
    }

    private void StopAttacking()
    {
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
}
