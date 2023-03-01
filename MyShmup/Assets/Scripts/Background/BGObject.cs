using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGObject : MonoBehaviour
{
    public bool _safeToTransition = false;
    public bool _loops;
    private bool _backgroundIsDynamic;
    public float _customSpeed;

    [SerializeField]
    private int _waypointIndex = 0;

    public bool _dying;
    public Transform _deathWaypoint;

    [System.Serializable]
    public class ScrollWaypointObject
    {
        public float _speed;
        public Transform _waypoint;
    }

    [SerializeField]
    private ScrollWaypointObject[] _waypoints;


    private void Start()
    {
        transform.position = _waypoints[_waypointIndex]._waypoint.position;
    }

    private void Update()
    {
        if(_dying && _safeToTransition)
        {
            MoveToDeath();
        }
        else
        {
            Move();
        }        
    }

    public void Move()
    {
        //if there are waypoints left
        if (_waypointIndex < _waypoints.Length)
        {
            if(!_backgroundIsDynamic)
            {
                //move to waypoint
                transform.position = Vector2.MoveTowards(transform.position, _waypoints[_waypointIndex]._waypoint.position,
                    _customSpeed * Time.deltaTime);
            }
            else
            {
                //move to waypoint
                transform.position = Vector2.MoveTowards(transform.position, _waypoints[_waypointIndex]._waypoint.position,
                    _waypoints[_waypointIndex]._speed * Time.deltaTime);
            }

            //TODO use lerp to transition speeds seamlessly/not using sudden speed changes

            //If reached waypoint
            if (transform.position == _waypoints[_waypointIndex]._waypoint.position)
            {
                //if last waypoint
                if(_waypointIndex == _waypoints.Length - 1)
                {
                    _safeToTransition = true;

                    if(!_backgroundIsDynamic && _dying)
                        //send message to gamemanager to transition to next bg 

                    //and loops
                    if (_loops)
                    {
                        //Go back to base position if not dead
                        if(!_dying)
                        {
                            _safeToTransition = false;
                            transform.position = _waypoints[0]._waypoint.position;
                        }
                    }
                    else
                    {
                        KillBG();
                    }
                }
                else
                {
                    //Go to next one
                    _waypointIndex++;
                }
            }
        }
    }

    public void MoveToDeath()
    {
        //move to waypoint
        transform.position = Vector2.MoveTowards(transform.position, _deathWaypoint.position,
            _waypoints[^1]._speed * Time.deltaTime);

        if (transform.position == _deathWaypoint.position)
        {
            Destroy(gameObject);
        }
    }

    public void KillBG()
    {
        _dying = true;
    }
}
