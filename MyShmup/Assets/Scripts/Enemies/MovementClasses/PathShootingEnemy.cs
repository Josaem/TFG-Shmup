using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathShootingEnemy : ShootingEnemy
{
    [SerializeField]
    private PathCreator _pathCreator;
    public EndOfPathInstruction _endOfPathInstruction;
    [SerializeField]
    private float _speed = 5;
    [SerializeField]
    private bool _dieOnLastWaypoint;

    private float distanceTravelled;

    protected override void Start()
    {
        base.Start();
        if (_pathCreator != null)
        {
            // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
            _pathCreator.pathUpdated += OnPathChanged;
        }
    }

    public override void Move()
    {
        if (_pathCreator != null)
        {
            distanceTravelled += _speed * Time.deltaTime;
            transform.position = _pathCreator.path.GetPointAtDistance(distanceTravelled, _endOfPathInstruction);
            transform.up = _pathCreator.path.GetDirectionAtDistance(distanceTravelled, _endOfPathInstruction);
        }

        if(distanceTravelled >= _pathCreator.path.length && _dieOnLastWaypoint)
        {
            DieByWaypoint();
        }
    }

    // If the path changes during the game, update the distance travelled so that the follower's position on the new path
    // is as close as possible to its position on the old path
    void OnPathChanged()
    {
        distanceTravelled = _pathCreator.path.GetClosestDistanceAlongPath(transform.position);
    }
}
