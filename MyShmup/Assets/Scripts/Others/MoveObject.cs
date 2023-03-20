using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveObject : MonoBehaviour
{
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

    // Update is called once per frame
    void Update()
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
            if (transform.position == _waypoints[_waypointToGo]._waypoint.position && _waypointToGo != _waypoints.Length - 1)
            { 
                //Go to next one
                _waypointToGo++;                
            }
        }
    }
}
