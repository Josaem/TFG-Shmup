using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGObject : MonoBehaviour
{
    [SerializeField]
    private GameObject _background;
    public bool _loops;
    public bool _backgroundIsDynamic;
    public float _customSpeed = 3;
    public Transform _deathWaypoint;
    [SerializeField]
    private ScrollWaypointObject[] _waypoints;
    
    [HideInInspector]
    public bool _dying = false;
    [HideInInspector]
    public  bool _safeToTransition = false;

    private int _waypointToGo = 0;

    private GameManager _myGM;


    [System.Serializable]
    public class ScrollWaypointObject
    {
        public float _speed;
        public Transform _waypoint;
    }


    private void Start()
    {
        _myGM = FindObjectsOfType<GameManager>()[0];
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
        if (_waypointToGo < _waypoints.Length)
        {
            if(!_backgroundIsDynamic)
            {
                //move to waypoint
                _background.transform.position = Vector2.MoveTowards(_background.transform.position, _waypoints[_waypointToGo]._waypoint.position,
                    _customSpeed * Time.deltaTime);
            }
            else
            {
                //move to waypoint
                _background.transform.position = Vector2.MoveTowards(_background.transform.position, _waypoints[_waypointToGo]._waypoint.position,
                    _waypoints[_waypointToGo]._speed * Time.deltaTime);
            }

            //If reached waypoint
            if (_background.transform.position == _waypoints[_waypointToGo]._waypoint.position)
            {
                //if last waypoint
                if(_waypointToGo == _waypoints.Length - 1)
                {
                    _safeToTransition = true;   

                    //and loops
                    if (_loops)
                    {
                        //Go back to base position if not dead
                        if(!_dying)
                        {
                            _safeToTransition = false;
                            _background.transform.position = _waypoints[0]._waypoint.position;
                            _waypointToGo = 1;
                        }
                    }
                    else
                    {
                        KillBG();
                    }

                    if (!_backgroundIsDynamic && _dying)
                    {
                        _myGM.AllowStartNextSection();
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

    public void MoveToDeath()
    {
        //move to waypoint
        if(!_backgroundIsDynamic)
        {
            _background.transform.position = Vector2.MoveTowards(_background.transform.position, _deathWaypoint.position,
                        _customSpeed * Time.deltaTime);
        }
        else
        {
            _background.transform.position = Vector2.MoveTowards(_background.transform.position, _deathWaypoint.position,
                        _waypoints[^1]._speed * Time.deltaTime);
        }

        if (_background.transform.position == _deathWaypoint.position)
        {
            Destroy(gameObject);
        }
    }

    public void KillBG()
    {
        _dying = true;
    }
}
