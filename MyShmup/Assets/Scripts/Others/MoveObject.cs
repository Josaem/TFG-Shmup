using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveObject : MonoBehaviour
{
    [SerializeField]
    private bool _repeatOnLastWaypoint;

    [SerializeField]
    private ScrollType _scrollType;

    [SerializeField]
    private float _scrollX;
    [SerializeField]
    private float _scrollY;

    [SerializeField]
    private ScrollWaypointObject[] _waypoints;

    private int _waypointToGo = 0;

    private enum ScrollType
    {
        Simple,
        Waypoint
    };

    [System.Serializable]
    private class ScrollWaypointObject
    {
        public float _speed;
        public Transform _waypoint;
    }

    void FixedUpdate()
    {
        switch(_scrollType)
        {
            case ScrollType.Simple: SimpleScroll();
                break;
            case ScrollType.Waypoint: WaypointScroll();
                break;
        }
    }

    private void SimpleScroll()
    {
        transform.position += new Vector3(-_scrollX * Time.deltaTime, -_scrollY * Time.deltaTime, 0);
    }

    private void WaypointScroll()
    {
        //if there are waypoints left
        if (_waypointToGo < _waypoints.Length)
        {
            
            //move to waypoint
            transform.position = Vector2.MoveTowards(transform.position, _waypoints[_waypointToGo]._waypoint.position,
                _waypoints[_waypointToGo]._speed * Time.deltaTime);
            

            //If reached waypoint and its not the last one
            if (transform.position == _waypoints[_waypointToGo]._waypoint.position)
            { 
                if (_waypointToGo < _waypoints.Length - 1)
                {
                    //Go to next one
                    _waypointToGo++;
                }
                else if (_repeatOnLastWaypoint)
                {
                    _waypointToGo = 0;
                }
            }
        }
    }

    public void SetSpeed(object[] tempStorage)
    {
        StartCoroutine(ChangeSpeed((float) tempStorage[0], (int) tempStorage[1], (float) tempStorage[2]));
    }

    private IEnumerator ChangeSpeed(float speed, int waypointToChange, float time)
    {
        yield return new WaitForSeconds(time);

        _waypoints[waypointToChange]._speed = speed;
    }
}
