using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AccelerateMoveableWaypoint : MonoBehaviour
{
    [SerializeField]
    private MoveObject _moveObject;
    [SerializeField]
    private float _newSpeed;
    [SerializeField]
    private int _waypointToChange;
    [SerializeField]
    private float _timeDelay;

    public void OnDestroy()
    {
        object[] tempStorage = new object[4];
        tempStorage[0] = _newSpeed;
        tempStorage[1] = _waypointToChange;
        tempStorage[2] = _timeDelay;

        _moveObject.SendMessage(nameof(_moveObject.SetSpeed), tempStorage, SendMessageOptions.DontRequireReceiver);
    }
}
