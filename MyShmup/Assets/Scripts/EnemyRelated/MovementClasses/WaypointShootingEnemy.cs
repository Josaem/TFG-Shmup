using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointShootingEnemy : ShootingEnemy
{
    [SerializeField]
    private ScrollWaypointObject[] _waypoints;
    [SerializeField]
    private bool _dieOnLastWaypoint = false;

    private int _waypointToGo = 0;

    [System.Serializable]
    public class ScrollWaypointObject
    {
        public float _speed;
        public Transform _waypoint;
    }

    public override void Move()
    {
        //if there are waypoints left
        if (_waypointToGo < _waypoints.Length)
        {

            //move to waypoint
            if(_waypoints[_waypointToGo]._waypoint != null)
                transform.position = Vector2.MoveTowards(transform.position, _waypoints[_waypointToGo]._waypoint.position,
                    _waypoints[_waypointToGo]._speed * Time.deltaTime);


            //If reached waypoint
            if (_waypoints[_waypointToGo]._waypoint != null && transform.position == _waypoints[_waypointToGo]._waypoint.position)
            {
                //and its the last one
                if (_waypointToGo == _waypoints.Length - 1)
                {
                    if (_dieOnLastWaypoint)
                    {
                        Kill();
                    }
                    else
                    {
                        _waypointToGo = 0;
                    }
                }
                else
                {
                    //Go to next one
                    _waypointToGo++;
                }
                
            }
        }
    }
}
