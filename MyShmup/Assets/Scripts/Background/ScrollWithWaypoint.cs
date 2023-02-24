using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollWithWaypoint : MonoBehaviour
{
    [SerializeField]
    private Transform _currentWaypoint;
    [SerializeField]
    private float _speed;

    [System.Serializable]
    public class ScrollWaypointObject
    {
        public float _speed;
        public Transform _waypoint;
    }

    [SerializeField]
    private ScrollWaypointObject[] _waypoints;

    private int index = 0;

    private void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    public void Move()
    {
        if(index < _waypoints.Length)
        {
            transform.position = Vector2.MoveTowards(transform.position, _waypoints[index]._waypoint.position,
                _waypoints[index]._speed * Time.deltaTime);

            if (transform.position == _waypoints[index]._waypoint.position)
            {
                index++;
            }
        }
    }
}
