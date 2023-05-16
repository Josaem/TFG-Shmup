using System;
using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _slowSpeed;
    [SerializeField] private float _fastSpeed;
    private Vector2 _movementValues;
    private bool _slowMovement = false;
    private bool _canMove = true;

    [Header("PlayerStatus")]
    [SerializeField] private float _deathTime;
    [SerializeField] private Transform _playerRespawnLocation;
    public Orientation shipOrientation = Orientation.horizontal;
    [SerializeField]
    private bool _invincibleTest = false;
    [HideInInspector]
    public bool _dead = false;

    [Header("General Shooting")]
    [SerializeField] private Transform _playerBulletPool;
    [SerializeField] private GameObject[] _optionsGameObject;
    [SerializeField] private LayerMask _enemyLayerMask;
    private InputAction fireAction;
    private bool _fireButtonPressed = false;
    private Vector2[] _optionsFollow;

    [Header("MainShot")]
    public int _1stShotScore = 1;
    [SerializeField] private float _fireActiveTime;
    [SerializeField] private float _fireRate1st;
    [SerializeField] private GameObject _primaryShotPrefab;
    [SerializeField] private Transform[] _base1stOptionsLocation;
    private bool _1stShotEnabled = false;
    private float _timeUntilShooting1st = 0;
    private float _fire1stTimeRemaining;

    [Header("SecondaryShot")]
    public int _2ndShotScore = 3;
    [SerializeField] private float _timeFor2ndShotActivation;
    [SerializeField] private float _fireRate2nd;
    [SerializeField] private float _noLockShotDistance;
    [SerializeField] private Transform _noLockObject;
    [SerializeField] private GameObject _secondaryShotPrefab;
    [SerializeField] private Transform[] _base2ndOptionsLocation;
    private bool _2ndShotEnabled = false;
    private float _timeUntilShooting2nd = 0;
    private float _timeUntil2ndShotEnabled;
    private GameObject _currentlyLockedEnemy;
    private int _gunToShoot = 0;
    private bool _2ndHasShot;

    [Header("Explosion")]
    [SerializeField]
    private float _exploCooldown = 4;
    private float _timeUntilExploAgain = 0;
    private InputAction explodeAction;

    [Header("Shield")]
    [SerializeField] private float _blockDuration;
    [SerializeField] private float _blockCooldown;
    [SerializeField] private Color _shieldOnCooldownColor;
    [SerializeField] private Color _shieldNotOnCooldownColor;
    private InputAction blockAction;
    private bool _invincible = false;
    private float _shieldRemainingTime = 0;
    private float _shieldRemainingCooldown = 0;
    private float _nextShieldCooldown;
    private bool _blockActionEnabled;


    [Header("Visuals")]
    [SerializeField] private SpriteRenderer _playerSprite;
    [SerializeField] private SpriteRenderer _hitboxSprite;
    private float _gunRotSpeed = 5f;
    private float _gunRotAmplitude = 0.5f;
    private float _gunRotAmplitudeOffset = 0.5f;
    private float _gunRotTime = 0;
    private SpriteRenderer[] _playerSprites;
    private Animator[] _optionsAnimator;
    private Animator _shipAnimator;
    private Animator _hitboxAnimator;

    [Header("Dependecies")]
    [SerializeField] private Rigidbody2D _rb;
    private PlayerInput playerInput;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        fireAction = playerInput.actions["Fire"];
        fireAction.ReadValue<float>();

        explodeAction = playerInput.actions["ExplodeShots"];
        explodeAction.ReadValue<float>();

        blockAction = playerInput.actions["Block"];
        blockAction.ReadValue<float>();

        _playerSprites = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        _timeUntil2ndShotEnabled = _timeFor2ndShotActivation;
        _optionsFollow = new Vector2[_base1stOptionsLocation.Length];

        fireAction.started += ctx => {
            if(!_dead)
            {
                _fireButtonPressed = true;
                StartShooting();
            }
        };

        fireAction.canceled += ctx => {
            _fireButtonPressed = false;
            CancelShooting();
        };

        explodeAction.started += ctx =>
        {
            if(Time.time > _timeUntilExploAgain && !_dead)
            {
                _timeUntilExploAgain = Time.time + _exploCooldown;
                ExplodeBullets();
            }
        };

        blockAction.started += ctx => {
            if(!_dead)
            {
                if (_shieldRemainingCooldown <= 0)
                    _blockActionEnabled = true;
                EnableBlock();
            }
        };

        blockAction.canceled += ctx => {
            _blockActionEnabled = false;

            if(_shieldRemainingTime > 0)
            {
                DisableBlock();
            }
        };

        _noLockObject.position = transform.right * _noLockShotDistance;

        _nextShieldCooldown = _blockCooldown;

        ChangeOrientation(shipOrientation);

        _optionsAnimator = new Animator[_optionsGameObject.Length];
        for (int i = 0; i < _optionsGameObject.Length; i++)
        {
            _optionsAnimator[i] = _optionsGameObject[i].GetComponentInChildren<Animator>();
        }
        _shipAnimator = _playerSprite.GetComponent<Animator>();
        _hitboxAnimator = _hitboxSprite.GetComponent<Animator>();

        DisableShip();
        HideShip();

        foreach (GameObject option in _optionsGameObject)
            option.transform.localPosition = Vector2.zero;

        HideOptions();
        HideHitbox();
        StartCoroutine(SpawnShip(_deathTime));
    }

    private void Update()
    {
        //Decrease time where fire is active 
        if (_fire1stTimeRemaining > 0) _fire1stTimeRemaining = Mathf.Max(_fire1stTimeRemaining - Time.deltaTime, 0f);
        if (_fire1stTimeRemaining > 0 && _timeUntil2ndShotEnabled > 0) Fire();

        //Decrease time until enabling the secondary shot 
        if (_timeUntil2ndShotEnabled > 0 && _fireButtonPressed) _timeUntil2ndShotEnabled = Mathf.Max(_timeUntil2ndShotEnabled - Time.deltaTime, 0f);
        if (_timeUntil2ndShotEnabled == 0 && _fireButtonPressed) Fire2nd();

        //Disable 1stshot enabled when stopped firing
        if (_fire1stTimeRemaining == 0 && _1stShotEnabled)
            _1stShotEnabled = false;

        //Decrease shield active time
        if (_shieldRemainingTime > 0) _shieldRemainingTime = Mathf.Max(_shieldRemainingTime - Time.deltaTime, 0f);

        //Decrease shield cooldown
        if (_shieldRemainingCooldown > 0) _shieldRemainingCooldown = Mathf.Max(_shieldRemainingCooldown - Time.deltaTime, 0f);

        //Disable blocking if shield time over, doesn't execute every frame
        if (_shieldRemainingTime <= 0 && _blockActionEnabled)
        {
            _blockActionEnabled = false;
            DisableBlock();
        }    

        //Change hitbox color depending on shield cooldown
        if (_shieldRemainingCooldown <= 0)
        {
            if(_hitboxSprite.color != _shieldNotOnCooldownColor)
            {
                _hitboxSprite.color = _shieldNotOnCooldownColor;
            }
        }
    }

    private void FixedUpdate()
    {
        if(_canMove)
            Move();
        else
            _rb.velocity = Vector2.zero;

        if (!_dead)
        {
            //If not shooting reset option positions
            if (!_1stShotEnabled && !_2ndShotEnabled)
            {
                ResetOptionPositions();
            }
            else
            {
                RotateOptions();
            } 
        }

        MoveOptions();
    }

    public void OnMove(InputValue value) => _movementValues = value.Get<Vector2>();

    private void Move()
    {
        if (_movementValues.x > 0.1)
            _movementValues.x = 1;
        if (_movementValues.x < - 0.1)
            _movementValues.x = -1;
        if (_movementValues.y > 0.1)
            _movementValues.y = 1;
        if (_movementValues.y < -0.1)
            _movementValues.y = -1;

        //TODOANIM play moving anim
        if (_slowMovement)
            _rb.velocity = _slowSpeed * Time.deltaTime * _movementValues.normalized;
        else
            _rb.velocity = _fastSpeed * Time.deltaTime * _movementValues.normalized;
    }

    private void StartShooting()
    {
        //Enable the timer to shoot both main and secondary
        _fire1stTimeRemaining = _fireActiveTime;
        _timeUntil2ndShotEnabled = _timeFor2ndShotActivation;
        _1stShotEnabled = true;
    }

    private void CancelShooting()
    {
        _slowMovement = false;
        _2ndShotEnabled = false;
        _currentlyLockedEnemy = null;
    }

    private void Fire()
    {
        if(!_dead)
        {
            _1stShotEnabled = true;

            //If cooldown between shots has passed
            if (Time.time > _timeUntilShooting1st)
            {
                _timeUntilShooting1st = Time.time + _fireRate1st;

                //For each option
                for (int i = 0; i < _optionsGameObject.Length; i++)
                {
                    //Shoot a shot
                    Instantiate(_primaryShotPrefab,
                        _optionsGameObject[i].transform.position, _base1stOptionsLocation[i].transform.rotation,
                        _playerBulletPool);

                    ShootAnimation(_optionsAnimator[i]);
                }
            }
        }
    }

    private void Fire2nd()
    {
        if (!_dead)
        {
            //Slow movement while 2nd shot
            if (_slowMovement == false) _slowMovement = true;

            _2ndShotEnabled = true;

            //If cooldown between shots has passed
            if (Time.time > _timeUntilShooting2nd)
            {
                _timeUntilShooting2nd = Time.time + _fireRate2nd;

                //Check front middle
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 20, _enemyLayerMask);
                RaycastHit2D hitBehind = Physics2D.Raycast(transform.position, -transform.right, 20, _enemyLayerMask);

                //for each option
                for (int i = 0; i < _optionsGameObject.Length; i++)
                {
                    if (_currentlyLockedEnemy == null)
                    {
                        if (hit.collider == null)
                        {
                            //Check front up
                            hit = Physics2D.Raycast(transform.TransformPoint(Vector3.up * transform.localScale.y / 2), Quaternion.AngleAxis(3, Vector3.forward) * transform.right, 20, _enemyLayerMask);

                            if (hit.collider == null)
                            {
                                //Check front down
                                hit = Physics2D.Raycast(transform.TransformPoint(-Vector3.up * transform.localScale.y / 2), Quaternion.AngleAxis(-3, Vector3.forward) * transform.right, 20, _enemyLayerMask);
                            }
                        }

                        if (hitBehind.collider == null)
                        {
                            //Check behind up
                            hitBehind = Physics2D.Raycast(transform.TransformPoint(Vector3.up * transform.localScale.y / 2), Quaternion.AngleAxis(-2, Vector3.forward) * -transform.right, 20, _enemyLayerMask);

                            if (hitBehind.collider == null)
                            {
                                //Check behind down
                                hitBehind = Physics2D.Raycast(transform.TransformPoint(-Vector3.up * transform.localScale.y / 2), Quaternion.AngleAxis(2, Vector3.forward) * -transform.right, 20, _enemyLayerMask);
                            }
                        }

                        if (hit.collider != null)
                        {
                            if (hitBehind.collider != null)
                            {
                                if (hit.distance - 0.5 <= hitBehind.distance)
                                {
                                    //Locked front only
                                    _currentlyLockedEnemy = hit.transform.gameObject;
                                    _optionsGameObject[i].transform.right = _currentlyLockedEnemy.transform.position - _optionsGameObject[i].transform.position;
                                }
                                else
                                {
                                    //Locked behind only
                                    _currentlyLockedEnemy = hitBehind.transform.gameObject;
                                    _optionsGameObject[i].transform.right = _currentlyLockedEnemy.transform.position - _optionsGameObject[i].transform.position;
                                }
                            }
                            else
                            {
                                //Locked front only
                                _currentlyLockedEnemy = hit.transform.gameObject;
                                _optionsGameObject[i].transform.right = _currentlyLockedEnemy.transform.position - _optionsGameObject[i].transform.position;
                            }
                        }
                        else if (hitBehind.collider != null)
                        {
                            //Locked behind only
                            _currentlyLockedEnemy = hitBehind.transform.gameObject;
                            _optionsGameObject[i].transform.right = _currentlyLockedEnemy.transform.position - _optionsGameObject[i].transform.position;
                        }
                        else
                        {
                            //hasn't hit anything both in front or behind, shoot ahead
                            //_optionsGameObject[i].transform.right = _noLockObject.transform.position - _optionsGameObject[i].transform.position;
                        }
                    }
                    else
                    {
                        //Point at locked target
                        _optionsGameObject[i].transform.right = _currentlyLockedEnemy.transform.position - _optionsGameObject[i].transform.position;
                    }
                    
                    if(_currentlyLockedEnemy != null && _gunToShoot == i && !_2ndHasShot)
                    {
                        //Shoot
                        Instantiate(_secondaryShotPrefab,
                            _optionsGameObject[i].transform.position, _optionsGameObject[i].transform.rotation,
                            _playerBulletPool);

                        ShootAnimation(_optionsAnimator[i]);

                        _2ndHasShot = true;

                        _gunToShoot++;

                        if(_gunToShoot >= _optionsGameObject.Length)
                            _gunToShoot = 0;
                    }
                }

                _2ndHasShot = false;
            }
        }
    }

    private void ResetOptionPositions()
    {
        for (int i = 0; i < _base1stOptionsLocation.Length; i++)
        {
            _optionsFollow[i] = _base1stOptionsLocation[i].transform.position;
            _optionsGameObject[i].transform.rotation = _base1stOptionsLocation[i].transform.rotation;
        }
    }

    private void RotateOptions()
    {
        _gunRotTime += Time.deltaTime;

        if(!_2ndShotEnabled)
        {
            
            _optionsFollow[0] = Vector3.Lerp(_base1stOptionsLocation[1].position,
                _base1stOptionsLocation[0].position,
                Mathf.Sin((_gunRotTime + 0.5f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset);

            _optionsFollow[1] = Vector3.Lerp(_base1stOptionsLocation[0].position,
                _base1stOptionsLocation[1].position,
                Mathf.Sin((_gunRotTime + 0.5f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset);

            _optionsFollow[2] = _base1stOptionsLocation[2].transform.position;

            _optionsFollow[3] = _base1stOptionsLocation[3].transform.position;

            for (int i = 0; i < _optionsGameObject.Length; i++)
            {
                _optionsGameObject[i].transform.rotation = _base1stOptionsLocation[i].transform.rotation;
            }
        }
        else
        {
            _optionsFollow[0] = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                Mathf.Sin((_gunRotTime + 0.25f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset);

            _optionsFollow[1] = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                Mathf.Sin((_gunRotTime + 0.5f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset);

            _optionsFollow[2] = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                Mathf.Sin((_gunRotTime + 0.75f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset);

            _optionsFollow[3] = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                Mathf.Sin((_gunRotTime + 1f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset);
        }
    }

    private void MoveOptions()
    {
        for (int i = 0; i < _optionsGameObject.Length; i++)
        {
            _optionsGameObject[i].transform.position = Vector3.Lerp(_optionsGameObject[i].transform.position, _optionsFollow[i], Time.deltaTime * 5);
        }
    }

    private void ExplodeBullets()
    {
        int enemiesKilled = 0;

        Enemy[] enemies = FindObjectsOfType<Enemy>();

        List<Enemy> enemiesToKillList = new List<Enemy>();
        List<Enemy> enemiesToDamageList = new List<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if(enemy.GetNails() != 0 || enemy.GetDrills() != 0)
            {
                if (enemy.GetIfDiesFromExplo())
                {
                    enemiesKilled++;
                    enemiesToKillList.Add(enemy);
                }
                else
                {
                    enemiesToDamageList.Add(enemy);
                }
                enemy.GetComponentInChildren<StuckBulletVisual>().GetComponentInChildren<Animator>().Play("WillDieFromExploExitAnim");
            }
        }

        //We explode bullets on enemies that will not die
        foreach (Enemy enemy in enemiesToDamageList)
        {
            if (enemy.GetNails() != 0 || enemy.GetDrills() != 0)
            {
                enemy.TakeDamageByExplosion(enemiesKilled);
            }
        }

        //We wait for anim to end before killing
        foreach (Enemy enemy in enemiesToKillList)
        {
            if (enemy.GetNails() != 0 || enemy.GetDrills() != 0)
            {
                StartCoroutine(DamageEnemiesByExplosion(enemy, enemiesKilled, _optionsAnimator[0].GetCurrentAnimatorStateInfo(0).length));
            }
        }
        
    }

    private IEnumerator DamageEnemiesByExplosion(Enemy enemy, int enemiesKilled, float time)
    {
        yield return new WaitForSeconds(time);

        if (enemy.GetNails() != 0 || enemy.GetDrills() != 0)
        {
            enemy.TakeDamageByExplosion(enemiesKilled);
        }
    }

    private void EnableBlock()
    {
        //if shield not on cooldown allow shielding for the next blockduration seconds
        if(_shieldRemainingCooldown <= 0)
        {
            _shieldRemainingTime = _blockDuration;
            _nextShieldCooldown = _blockCooldown;
            _invincible = true;
            EnableBlockVisual();
        }
    }

    private void DisableBlock()
    {
        DisableBlockVisual();
        _shieldRemainingTime = 0;
        _shieldRemainingCooldown = _nextShieldCooldown;
        _invincible = false;

        _hitboxSprite.color = _shieldOnCooldownColor;
    }

    public void ResetShield(float cooldownDivider)
    {
        //if shield is up
        if(_shieldRemainingTime > 0)
        {
            _nextShieldCooldown = Mathf.Min(_nextShieldCooldown, _blockCooldown/cooldownDivider);
        }
    }

    private IEnumerator StopInvincibility(float time)
    {
        yield return new WaitForSeconds(time);

        _invincible = false;
    }

    public void GetHurt()
    {
        if(!_invincible && !_dead && !_invincibleTest)
        {
            GameProperties._life--;
            FindObjectOfType<LifeUI>().SetUpLifebar();

            DisableShip();

            OptionsToCenter();
            foreach(Animator option in _optionsAnimator)
            {
                option.Play("OptionDespawnAnim");
            }
            Invoke(nameof(HideOptions), _optionsAnimator[0].GetCurrentAnimatorStateInfo(0).length);

            _hitboxAnimator.Play("PlayerHitboxDespawnAnim");
            Invoke(nameof(HideHitbox), _hitboxAnimator.GetCurrentAnimatorStateInfo(0).length);

            _shipAnimator.Play("ShipDeathAnim");
            Invoke(nameof(HideShip), _shipAnimator.GetCurrentAnimatorStateInfo(0).length);

            StartCoroutine(SpawnShip(_deathTime));
            
            //TODOUI Use Credit screen
        }
    }

    private void DisableShip()
    {
        _invincible = true;
        _dead = true;
        CancelShooting();
        _canMove = false;
    }

    private void HideHitbox()
    {
        _hitboxSprite.enabled = false;
    }

    private void ShowHitbox()
    {
        _hitboxSprite.enabled = true;
        _hitboxAnimator.Play("PlayerHitboxSpawnAnim");
    }

    private IEnumerator SpawnShip(float time)
    {
        yield return new WaitForSeconds(1.5f);

        ShowShip();

        yield return new WaitForSeconds(_shipAnimator.GetCurrentAnimatorStateInfo(0).length);

        _canMove = true;
        _dead = false;

        ShowHitbox();
        ShowOptions();

        yield return new WaitForSeconds(time);
        _invincible = false;
        _shipAnimator.Play("PlayerIdleAnim");
    }

    private void HideShip()
    {
        transform.position = new Vector2(-15, 15);
    }

    private void OptionsToCenter()
    {
        foreach (Vector2 optionFollow in _optionsFollow)
        {
            for (int i = 0; i < _base1stOptionsLocation.Length; i++)
            {
                _optionsFollow[i] = transform.position;
            }
        }
    }

    private void HideOptions()
    {
        foreach (GameObject option in _optionsGameObject)
        {
            option.GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
    }

    private void ShowOptions()
    {
        foreach (GameObject option in _optionsGameObject)
        {
            option.transform.localPosition = Vector2.zero;
            option.GetComponentInChildren<SpriteRenderer>().enabled = true;
            option.GetComponentInChildren<Animator>().Play("OptionSpawnAnim");
        }
    }

    private void ShowShip()
    {
        transform.position = _playerRespawnLocation.position;

        _shipAnimator.Play("PlayerEntryAnim");
    }

    private void EnableBlockVisual()
    {
        if(!_dead)
        {
            _shipAnimator.Play("PlayerShieldAnim");
        }
    }

    private void DisableBlockVisual()
    {
        if(!_dead)
        {
            if (!_shipAnimator.GetCurrentAnimatorStateInfo(0).IsName("Out") && !_shipAnimator.IsInTransition(0))
                _shipAnimator.Play("PlayerShieldOutAnim");
        }
    }

    public void ChangeOrientation(Orientation orientation)
    {
        shipOrientation = orientation;

        //add here the visual changes and the lerping
        switch(orientation)
        {
            case Orientation.diagonalUp:
                transform.rotation = Quaternion.Euler(0, 0, 45);
                break;
            case Orientation.diagonalDown:
                transform.rotation = Quaternion.Euler(0, 0, -45);
                break;
            case Orientation.vertical:
                transform.rotation = Quaternion.Euler(0, 0, 90);
                break;
            case Orientation.horizontal:
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
        }
    }

    private void ShootAnimation(Animator gun)
    {
        gun.Play("GunShooting");
    }
}
