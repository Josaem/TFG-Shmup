using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [Header("Status")]
    [SerializeField]
    private int _health;

    [Header("Wave Dependent")]
    public bool _prioritary = false;
    public bool _multiphase = false;

    [Header("Extra")]
    [SerializeField]
    private int _specialRequirementsIndex = 0;
    [SerializeField]
    private float _delayUntilFirstAttack = 0;

    [Header("Movement")]
    [SerializeField]
    private WaypointMovement _entryDestination;
    [SerializeField]
    private WaypointMovement _exitDestination;

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
    private void Update()
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
    public void MoveToInitialPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, _entryDestination._waypoint.position,
                    _entryDestination._speed * Time.deltaTime);

        if (transform.position == _entryDestination._waypoint.position)
        {
            Debug.Log("Reached Initial Position");
            _movementState = EnemyMovementState.Moving;
            Invoke(nameof(StartAttacking), _delayUntilFirstAttack);
        }
    }

    public virtual void StartAttacking()
    {

    }

    public virtual void Move()
    {

    }

    public void Kill()
    {
        _movementState = EnemyMovementState.Dying;
    }

    public void TakeDamage()
    {
        /*
        TODO
        count of drills and shots personal++ -> depending on what hit
        count of drills and shots++ -> gamepropreties
        
        if bigEnemy tag drillshots stick

        health -= shotDamage

        UpdateHealth
         */
    }

    public void TakeDamageByExplosion()
    {
        /*
        TODO
        life - (drills(drill damage reduction) * shots(shot damage reduction)
        if life = 0
            DieByExplosion
        */
    }

    public void UpdateHealth()
    {
        /*
        if health 20 % show it
        if health 10 % show it
        if health 5 % show it
        if health 0 or less
            enemiesstuck--
            AddScore
            Die
        */
    }

    public void WillDieWithExplosion()
    {
        //TODO check if life <= 0 if exploded
    }

    private void Die()
    {
        if(_myWave != null)
        {
            if(_prioritary && !_multiphase)
            {
                _myWave.PriorityEnemyKilled();
            }

            if(_multiphase)
            {
                //TODO add multiphase scrip, call to it, spawn respective enemy and if last phase set multiphase to false
            }

            if(_specialRequirementsIndex != 0)
            {
                GameProperties._extraLevelRequirements[GameProperties._currentLevel][_specialRequirementsIndex]--;
            }
        }

        //Animate death

        Destroy(gameObject);
    }

    private void AddScore()
    {
        //Add GameProperties score += enemyValue * drill * shots 
    }

    public void MoveToDeath()
    {
        transform.position = Vector2.MoveTowards(transform.position, _exitDestination._waypoint.position,
                    _exitDestination._speed * Time.deltaTime);

        if (transform.position == _exitDestination._waypoint.position)
        {
            DieByWaypoint();
        }
    }

    private void DieByWaypoint()
    {
        //gamepropreties count of shots -= current shot count
        //gamepropreties count of drills -= current drill count 
        Destroy(gameObject);
    }

    /*
    TODO
    Player explosion
        enemyKill = 0
        accumulated shots = 0
        accumulated drills = 0

        look in area for enemies
            if shot count != 0 || drill count != 0
                check if kill
                    enemyKill++
                    accumulated shots += enemy shots
                    accumulated drills += enemy drills
                enemy Take Damage By explosion
        add score * drill * shots * kill    
     */
}
