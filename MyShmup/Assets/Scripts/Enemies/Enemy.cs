using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [Header("Status")]
    [SerializeField]
    private int _health;
    public bool _prioritary = false;

    [Header("Extra")]
    [SerializeField]
    private int _specialRequirementsIndex = 0;
    [SerializeField]
    private float _delayUntilFirstAttack = 0;

    [Header("Movement")]
    [SerializeField]
    private WaypointMovement _entryDestination;
    [SerializeField]
    private WaypointMovement _deathDestination;

    private WaveObject _myWave;
    private EnemyMovementState _movementState = EnemyMovementState.Entering;
    private bool _invincible = false;

    [System.Serializable]
    private class WaypointMovement
    {
        public float _speed;
        public Transform _waypoint;
    }

    private enum EnemyMovementState
    {
        Entering,
        Moving,
        Dying,
    };

    // Start is called before the first frame update
    void Start()
    {
        _myWave = GetComponentInParent<WaveObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(_movementState == EnemyMovementState.Entering)
        {
            MoveToInitialPosition();
        }
        if (_movementState == EnemyMovementState.Moving)
        {
            Move();
        }
        if (_movementState == EnemyMovementState.Dying)
        {
            MoveToDeath();
        }
    }

    public void Kill()
    {
        _movementState = EnemyMovementState.Dying;
    }

    public void MoveToInitialPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, _entryDestination._waypoint.position,
                    _entryDestination._speed * Time.deltaTime);

        if (transform.position == _deathDestination._waypoint.position)
        {
            _movementState = EnemyMovementState.Moving;
        }
    }

    public void Move()
    {
        
    }

    public void MoveToDeath()
    {
        transform.position = Vector2.MoveTowards(transform.position, _deathDestination._waypoint.position,
                    _deathDestination._speed * Time.deltaTime);

        if (transform.position == _deathDestination._waypoint.position)
        {
            DieByWaypoint();
        }
    }

    private void Die()
    {
        if(_myWave != null)
        {
            if(_prioritary)
            {
                _myWave.PriorityEnemyKilled();
            }
        }
        
        //Animate death
        Destroy(gameObject);
    }

    private void DieByWaypoint()
    {
        Destroy(gameObject);
    }

    /*
    Enemies (clase base):
    -behavior
    -path
    -waypoints
    -offset hasta primer ataque
    -attack[]:
        -weapon[]
        -duration
        -timeUntilNextAttack
    
    Move
    GameManage.UpdatePriorityEnemies -> when dead
    Update levelRequirements
    Die
    ShootAttack
    */
}
