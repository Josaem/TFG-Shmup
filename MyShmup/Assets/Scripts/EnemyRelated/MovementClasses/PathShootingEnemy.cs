using PathCreation;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathShootingEnemy : ShootingEnemy
{
    [SerializeField]
    private PathCreator _pathCreator;
    public EndOfPathInstruction _endOfPathInstruction;
    [SerializeField]
    private float _speed = 5;
    [SerializeField]
    private bool _rotateTowardsPath = true;
    [SerializeField]
    private bool _dieOnLastWaypoint;

    private float distanceTravelled;
    private VertexPath _path;
    private Transform _transform;

    protected override void Start()
    {
        base.Start();
        _transform = transform;
        if (_pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            _pathCreator.pathUpdated += OnPathChanged;
            _path = _pathCreator.path;
        }
    }

    public override void Move()
    {
        if (_pathCreator != null && _pathCreator.path != null)
        {
            distanceTravelled += _speed * Time.deltaTime;
            _transform.position = _path.GetPointAtDistance(distanceTravelled, _endOfPathInstruction);
            if (_rotateTowardsPath)
            {
                _transform.up = _path.GetDirectionAtDistance(distanceTravelled, _endOfPathInstruction);
            }
        }

        if(distanceTravelled >= _path.length && _dieOnLastWaypoint)
        {
            DieByWaypoint();
        }
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        distanceTravelled = _path.GetClosestDistanceAlongPath(transform.position);
    }
}
