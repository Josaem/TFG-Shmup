using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    [Header("Status")]
    public int _maxHealth;
    [SerializeField]
    private int _baseScore;
    public int _stuckNails = 0;
    public int _stuckDrills = 0;
    private bool _isDead;

    [Header("Wave Dependent")]
    public bool _prioritary = false;
    [SerializeField]
    private WaveSelector _dieOnWave;
    private GameManager _myGM;

    [Header("Extra")]
    [SerializeField]
    private int _specialRequirementsIndex = 0;
    [SerializeField]
    protected float _delayUntilActive = 0;
    [SerializeField]
    protected float _delayUntilFirstAction = 0;
    [SerializeField]
    protected bool _spawnInBackground;
    [SerializeField]
    protected bool _dieInBackground;
    [SerializeField]
    private Phase _nextPhase;
    [SerializeField]
    private GameObject[] _stuffToSpawnOnDeath;
    [SerializeField]
    private Enemy[] _enemiesToKillOnDeath;
    [SerializeField]
    private GameObject _collisionsWithPlayer;

    [Header("Movement")]
    [SerializeField]
    protected WaypointMovement _entryDestination;
    [SerializeField]
    private WaypointMovement _exitDestination;

    protected int _currentHealth;
    protected WaveObject _myWave;
    protected EnemyMovementState _movementState = EnemyMovementState.Unspawned;
    [SerializeField]
    protected bool _invincible = false;

    [System.Serializable]
    protected class WaypointMovement
    {
        public float _speed;
        public Transform _waypoint;
    }

    [System.Serializable]
    protected class Phase
    {
        public GameObject _phaseObject;
        public bool _entryPosDifferent;
    }

    public enum EnemyMovementState
    {
        Entering,
        Moving,
        Dying,
        Unspawned
    };

    [System.Serializable]
    private class WaveSelector
    {
        public int _section;
        public int _wave;
    }

    // Start is called before the first frame update
    protected void Start()
    {
        Invoke(nameof(Spawn), _delayUntilActive);
        _myWave = GetComponentInParent<WaveObject>();
        _currentHealth = _maxHealth;
        UpdateHealth();
        _myGM = FindObjectOfType<GameManager>();
    }

    protected virtual void Spawn()
    {
        _movementState = EnemyMovementState.Entering;

        if (_spawnInBackground)
        {
            _invincible = true;
            GetComponent<BoxCollider2D>().enabled = false;
            if(_collisionsWithPlayer != null)
                _collisionsWithPlayer.SetActive(false);
        }
    }

    // Update is called once per frame
    protected virtual void Update()
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

        if (_myGM != null && (_myGM._sectionIndex == _dieOnWave._section && _myGM._waveIndex == _dieOnWave._wave))
        {
            Kill();
        }
    }

    public void MoveToInitialPosition()
    {
        transform.position = Vector2.MoveTowards(transform.position, _entryDestination._waypoint.position,
                    _entryDestination._speed * Time.deltaTime);

        if (transform.position == _entryDestination._waypoint.position)
        {
            if (_spawnInBackground)
            {
                _invincible = false;
                GetComponent<BoxCollider2D>().enabled = true;
                _collisionsWithPlayer.SetActive(true);
            }

            _movementState = EnemyMovementState.Moving;
            Invoke(nameof(StartAction), _delayUntilFirstAction);
        }
    }

    public virtual void StartAction()
    {

    }

    public virtual void Move()
    {

    }

    public virtual void Kill()
    {
        if ((_dieOnWave._section == 0 && _dieOnWave._wave == 0)
            || (_myGM._sectionIndex == _dieOnWave._section && _myGM._waveIndex == _dieOnWave._wave))
        {
            _movementState = EnemyMovementState.Dying;
            if(_dieInBackground)
            {
                _invincible = true;
                GetComponent<BoxCollider2D>().enabled = false;
                if (_collisionsWithPlayer != null)
                    _collisionsWithPlayer.SetActive(false);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if(!_invincible && _movementState != EnemyMovementState.Unspawned)
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
    }

    public void TakeDamageByExplosion()
    {
        /*
        TODO
        life - (drills(drill damage reduction) * shots(shot damage reduction)
        if life = 0
            DieByExplosion
        */

        UpdateHealth();
    }

    public void UpdateHealth()
    {
        float currentHealthPercent = (_currentHealth * 100f)/_maxHealth;

        if(currentHealthPercent <= 20)
        {
            switch (currentHealthPercent)
            { 
                case <= 5:
                    break;
                case <= 10:
                    break;
                case <= 20:
                    //TODO visuales
                    break;
            }
        }

        if(_currentHealth <= 0)
        {
            //enemiesStuck--
            AddScore();
            Die();
        }
    }

    public void WillDieWithExplosion()
    {
        //TODO check if life <= 0 if exploded
    }

    protected virtual void Die()
    {
        if(!_isDead)
        {
            _isDead = true;
            if (_nextPhase._phaseObject != null)
            {
                SpawnPhase();
            }
            else
            {
                if (_myWave != null && _prioritary)
                {
                    _myWave.SetPriorityEnemiesDead();
                }

                if (_specialRequirementsIndex != 0)
                {
                    GameProperties._extraLevelRequirements[GameProperties._currentLevel][_specialRequirementsIndex]--;
                }
            }

            foreach (GameObject spawn in _stuffToSpawnOnDeath)
            {
                if (spawn.GetComponent<WaveGunNoEnemy>() != null)
                {
                    Instantiate(spawn, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(spawn);
                }
            }

            foreach (Enemy enemy in _enemiesToKillOnDeath)
            {
                if (enemy != null)
                {
                    enemy.Kill();
                }
            }

            //Animate death
            GetComponent<BoxCollider2D>().enabled = false;
            Destroy(gameObject);
        }
    }

    private void SpawnPhase()
    {
        GameObject nextPhase;

        if (_myWave != null)
        {
            nextPhase = Instantiate(_nextPhase._phaseObject,
                Vector2.zero, Quaternion.identity, _myWave.transform);
        }
        else
        {
            nextPhase = Instantiate(_nextPhase._phaseObject,
                Vector2.zero, Quaternion.identity);
        }

        Enemy[] enemies = nextPhase.GetComponentsInChildren<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (!_nextPhase._entryPosDifferent)
            {
                enemy._entryDestination._waypoint.position = transform.position;
            }
            enemy.transform.position = transform.position;
        }
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
        Die();
    }

    public void IsShielded()
    {
        _invincible = true;
    }

    public void IsNotShielded()
    {
        _invincible = false;
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
