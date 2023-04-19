using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotativeBullet : ProyectileBehavior
{
    [SerializeField]
    protected float _rot = 0;

    protected override void Update()
    {
        RotateMov();
    }

    protected virtual void RotateMov()
    {
        transform.Rotate(0, 0, _rot);
    }
}
