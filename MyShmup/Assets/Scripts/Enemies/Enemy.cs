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
    private bool _willDieByExplosion;
    private int _stuckNails = 0;
    private int _stuckDrills = 0;
    private bool _isDead;
    private bool _deadByWaypoint;
    private int _accumulatedScore;

    [Header("Wave Dependent")]
    public bool _prioritary = false;
    [SerializeField]
    private WaveSelector _dieOnWave;
    private GameManager _myGM;

    [Header("Extra")]
    [SerializeField]
    private int _specialRequirementsIndex = 0;
    public float _delayUntilActive = 0;
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
    private bool _clearBulletsOnDeath;
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
    protected bool _shielded = false;
    private StuckBulletVisual _stuckBulletVisual;
    private PlayerController _player;

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
        UpdateHealthVisuals();
        _myGM = FindObjectOfType<GameManager>();
        _stuckBulletVisual = GetComponentInChildren<StuckBulletVisual>();
        _player = FindObjectOfType<PlayerController>();

        if (_spawnInBackground)
        {
            GetComponent<Collider2D>().enabled = false;
            if (_collisionsWithPlayer != null)
                _collisionsWithPlayer.SetActive(false);
        }
    }

    protected virtual void Spawn()
    {
        _movementState = EnemyMovementState.Entering;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        switch(_movementState)
        {
            case EnemyMovementState.Entering:
                if(_entryDestination._waypoint != null)
                {
                    MoveToInitialPosition();
                }
                else
                {
                    MovedToInitialPosition();
                }
                break;
            case EnemyMovementState.Moving:
                Move();
                break;
            case EnemyMovementState.Dying:
                if (_exitDestination._waypoint != null)
                {
                    MoveToDeath();
                }
                else
                {
                    DieByWaypoint();
                }
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
                GetComponent<Collider2D>().enabled = true;
                if(_collisionsWithPlayer != null)
                    _collisionsWithPlayer.SetActive(true);
            }

            _movementState = EnemyMovementState.Moving;
            Invoke(nameof(StartAction), _delayUntilFirstAction);
        }
    }

    public void MovedToInitialPosition()
    {
        if (_spawnInBackground)
        {
            GetComponent<Collider2D>().enabled = true;
            if (_collisionsWithPlayer != null)
                _collisionsWithPlayer.SetActive(true);
        }

        _movementState = EnemyMovementState.Moving;
        Invoke(nameof(StartAction), _delayUntilFirstAction);
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
                GetComponent<Collider2D>().enabled = false;
                if (_collisionsWithPlayer != null)
                    _collisionsWithPlayer.SetActive(false);
            }
        }
    }

    public int GetNails()
    {
        return _stuckNails;
    }

    public int GetDrills()
    {
        return _stuckDrills;
    }

    public bool GetIfDiesFromExplo()
    {
        return _willDieByExplosion;
    }

    public void TakeDamage(int damage, bool shotIsPrimary, Vector3 bulletPos)
    {
        if (shotIsPrimary)
        {
            _stuckNails++;
        }
        else
        {
            _stuckDrills++;
        }

        UpdateScore(0);

        _stuckBulletVisual.GotHit(shotIsPrimary, bulletPos, _accumulatedScore);

        bool currentExploDeadStatus = _willDieByExplosion;
        WillEnemyDieByExplosion();

        if(_willDieByExplosion == true && currentExploDeadStatus == false)
        {
            _stuckBulletVisual.EnemyWillDieByExplosion();
        }

        if (!_invincible && !_shielded)
        {
            _currentHealth -= damage;

            UpdateHealthVisuals();

            if (_currentHealth <= 0)
            {
                Die();
            }
        }
    }

    public void TakeDamageByExplosion(int deadAmount)
    {
        if (!_isDead)
        {
            float damage = 0.40f * (_stuckNails + _stuckDrills);

            if (_willDieByExplosion)
            {
                UpdateScore(deadAmount);
                _stuckBulletVisual.GotExploded(true, deadAmount, _accumulatedScore);
                Die();
            }
            else
            {
                _stuckNails = 0;
                _stuckDrills = 0;

                UpdateScore(0);
                _stuckBulletVisual.GotExploded(false, deadAmount, _accumulatedScore);

                bool currentExploDeadStatus = _willDieByExplosion;
                WillEnemyDieByExplosion();

                if (_willDieByExplosion == true && currentExploDeadStatus == false)
                {
                    _stuckBulletVisual.EnemyWillDieByExplosion();
                }
                _currentHealth -= (int) damage;

                UpdateHealthVisuals();
            }
        }
    }

    public void WillEnemyDieByExplosion()
    {
        float damage = 0.40f * (_stuckNails + _stuckDrills);
        if (_currentHealth - damage <= 0)
        {
            _willDieByExplosion = true;
        }
        else
        {
            _willDieByExplosion = false;
        }

    }

    public void UpdateHealthVisuals()
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

            if(!_deadByWaypoint)
            {
                AddScore();
                //Animate death
            }

            if(_clearBulletsOnDeath)
            {
                _myWave._bulletPool.ClearBullets();
            }

            GetComponent<Collider2D>().enabled = false;
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
        GameProperties._score += _accumulatedScore;
        //TODO find score text and update
    }

    private void UpdateScore(int deadMultiplier)
    {
        int nails = _stuckNails;
        int drills = _stuckDrills;
        int nailScore = _player._1stShotScore;
        int drillScore = _player._2ndShotScore;
        int deadAmount = deadMultiplier;

        if (_stuckNails == 0)
        {
            nails = 1;
            nailScore = 1;
        }

        if(_stuckDrills == 0)
        {
            drills = 1;
            drillScore = 1;
        }

        if (deadMultiplier == 0)
        {
            deadAmount = 1;
        }

        _accumulatedScore = _baseScore * (nails * nailScore) * (drills * drillScore) * deadAmount;    
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
        _deadByWaypoint = true;
        Die();
    }

    public void IsShielded()
    {
        _shielded = true;
    }

    public void IsNotShielded()
    {
        _shielded = false;
    }
}
