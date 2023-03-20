using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [Header("Status")]
    public int _maxHealth;
    public int _currentHealth;
    [SerializeField]
    private int _baseScore;
    public int _stuckNails = 0;
    public int _stuckDrills = 0;

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
    protected EnemyMovementState _movementState = EnemyMovementState.Entering;
    private bool _invincible = false;

    [System.Serializable]
    private class WaypointMovement
    {
        public float _speed;
        public Transform _waypoint;
    }

    public enum EnemyMovementState
    {
        Entering,
        Moving,
        Dying,
    };

    // Start is called before the first frame update
    void Start()
    {
        _myWave = GetComponentInParent<WaveObject>();
        _currentHealth = _maxHealth;
        UpdateHealth();
    }

    // Update is called once per frame
    private void Update()
    {
        switch(_movementState)
        {
            case EnemyMovementState.Entering:
                MoveToInitialPosition();
                break;
            case EnemyMovementState.Moving:
                Move();
                break;
            case EnemyMovementState.Dying:
                MoveToDeath();
                break;
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

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        UpdateHealth();
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
        float currentHealthPercent = (_currentHealth * 100f)/_maxHealth;
        Debug.Log(currentHealthPercent);

        if(currentHealthPercent <= 20)
        {
            switch (currentHealthPercent)
            {
                case <= 0:
                    //enemiesStuck--
                    AddScore();
                    Die();
                    break;
                case <= 5:
                    break;
                case <= 10:
                    break;
                case <= 20:
                    //TODO visuales
                    break;
            }
        }
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
