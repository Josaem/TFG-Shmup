using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMovShootingEnemy : ShootingEnemy
{
    [SerializeField]
    private ScrollWaypointObject[] _waypoints;
    [SerializeField]
    private bool _dieOnLastWaypoint = false;

    private int _waypointToGo = 0;
    private bool _firstWaypoint = true;

    [System.Serializable]
    private class ScrollWaypointObject
    {
        public float _speed;
        public Transform _waypoint;
    }

    public override void Move()
    {
        if (_firstWaypoint)
        {
            //move to waypoint
            transform.position = Vector2.MoveTowards(transform.position, _waypoints[0]._waypoint.position,
                _entryDestination._speed * Time.deltaTime);

            //If reached waypoint
            if (transform.position == _waypoints[0]._waypoint.position)
            {
                _waypointToGo = Random.Range(0, _waypoints.Length);
                _firstWaypoint = false;
            }
        }
        else
        {
            //if there are waypoints left
            if (_waypointToGo < _waypoints.Length)
            {

                //move to waypoint
                transform.position = Vector2.MoveTowards(transform.position, _waypoints[_waypointToGo]._waypoint.position,
                    _waypoints[_waypointToGo]._speed * Time.deltaTime);


                //If reached waypoint
                if (transform.position == _waypoints[_waypointToGo]._waypoint.position)
                {
                    _waypointToGo = Random.Range(0, _waypoints.Length);
                }
            }
        }
       
        /*
        TODO
        -behavior
        -waypoints
         */
    }
}
