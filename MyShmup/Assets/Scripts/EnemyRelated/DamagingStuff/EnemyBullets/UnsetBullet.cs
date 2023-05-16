using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UnsetBullet : ProyectileBehavior
{
#if UNITY_EDITOR
    [Help("Only used maxDistance, which determines the time to die", UnityEditor.MessageType.None)]
#endif
    public bool _useMaxDistance = false;

    public override void Move(float speed)
    {
    }

    public override void SetDeath(float maxDistance)
    {
        if(_useMaxDistance)
        {
            _maxDistance = maxDistance;
            Invoke(nameof(Die), maxDistance);
        }
    }
}
