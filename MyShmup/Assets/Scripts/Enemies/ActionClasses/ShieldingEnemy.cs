using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldingEnemy : Enemy
{
    [SerializeField]
    private GameObject _shieldZone;

    protected override void Start()
    {
        base.Start();
        _shieldZone.SetActive(false);
    }
    public override void StartAction()
    {
        //Play animation for enemyshield
        _shieldZone.SetActive(true);
    }

    public override void Kill()
    {
        base.Kill();
        _shieldZone.SetActive(false);
    }
}
