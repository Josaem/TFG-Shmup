using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

/*
    TODO TAGS:

    TODOVISUALS
    TODOUI
    TODO
 */

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _slowSpeed;
    [SerializeField] private float _fastSpeed;
    private Vector2 _movementValues;
    private bool _slowMovement = false;

    [Header("PlayerStatus")]
    [SerializeField] private float _deathTime;
    [SerializeField] private Transform _playerRespawnLocation;
    private bool _dead = false;

    [Header("General Shooting")]
    [SerializeField] private Transform _playerBulletPool;
    [SerializeField] private GameObject[] _optionsGameObject;
    [SerializeField] private LayerMask _enemyLayerMask;
    private InputAction fireAction;
    private bool _fireButtonPressed = false;

    [Header("MainShot")]
    [SerializeField] private float _fireActiveTime;
    [SerializeField] private float _fireRate1st;
    [SerializeField] private GameObject _primaryShotPrefab;
    [SerializeField] private Transform[] _base1stOptionsLocation;
    private bool _1stShotEnabled = false;
    private float _timeUntilShooting1st = 0;
    private float _fire1stTimeRemaining;

    [Header("SecondaryShot")]
    [SerializeField] private float _timeFor2ndShotActivation;
    [SerializeField] private float _fireRate2nd;
    [SerializeField] private float _noLockShotDistance;
    [SerializeField] private GameObject _secondaryShotPrefab;
    [SerializeField] private Transform[] _base2ndOptionsLocation;
    private bool _2ndShotEnabled = false;
    private float _timeUntilShooting2nd = 0;
    private float _timeUntil2ndShotEnabled;
    private GameObject _currentlyLockedEnemy;

    [Header("Shield")]
    [SerializeField] private float _blockDuration;
    [SerializeField] private float _blockCooldown;
    [SerializeField] private Color _shieldOnCooldownColor;
    [SerializeField] private Color _shieldNotOnCooldownColor;
    private InputAction blockAction;
    private bool _invincible = false;
    private float _shieldRemainingCooldown = 0;


    [Header("Visuals")]
    [SerializeField] private GameObject _shieldSprite;
    [SerializeField] private GameObject _hitboxSprite;
    private float _gunRotSpeed = 5f;
    private float _gunRotAmplitude = 0.5f;
    private float _gunRotAmplitudeOffset = 0.5f;
    private float _gunRotTime = 0;
    private SpriteRenderer[] _playerSprites;
    private SpriteRenderer _hitboxRenderer;

    [Header("Dependecies")]
    [SerializeField] private Rigidbody2D _rb;
    private PlayerInput playerInput;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        fireAction = playerInput.actions["Fire"];
        fireAction.ReadValue<float>();

        blockAction = playerInput.actions["Block"];
        blockAction.ReadValue<float>();

        _playerSprites = GetComponentsInChildren<SpriteRenderer>();
        _hitboxRenderer = _hitboxSprite.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        _timeUntil2ndShotEnabled = _timeFor2ndShotActivation;
        ResetOptionPositions();

        fireAction.started += ctx => {
            _fireButtonPressed = true;
            StartShooting();
        };
        fireAction.canceled += ctx => {
            _fireButtonPressed = false;
            CancelShooting();
        };

        blockAction.performed += ctx => {
            Block();
        };
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

        //Decrease shield cooldown
        if (_shieldRemainingCooldown > 0) _shieldRemainingCooldown = Mathf.Max(_shieldRemainingCooldown - Time.deltaTime, 0f);

        //Change hitbox color depending on shield cooldown
        if (_shieldRemainingCooldown == 0 && _hitboxRenderer.color != _shieldNotOnCooldownColor) _hitboxRenderer.color = _shieldNotOnCooldownColor;

        //If not shooting reset option positions
        if (!_1stShotEnabled && !_2ndShotEnabled)
        {
            ResetOptionPositions();
            _gunRotTime = 0;
        }
        else
        {
            RotateOptions();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    public void OnMove(InputValue value) => _movementValues = value.Get<Vector2>();

    private void Move()
    {
        if (!_dead)
        {
            //TODOANIM play moving anim
            if (_slowMovement)
                _rb.velocity = _slowSpeed * Time.deltaTime * _movementValues;
            else
                _rb.velocity = _fastSpeed * Time.deltaTime * _movementValues;
        }
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
                        _optionsGameObject[i].transform.position, _optionsGameObject[i].transform.rotation,
                        _playerBulletPool);
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

                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 15, _enemyLayerMask);

                //for each option
                for (int i = 0; i < _optionsGameObject.Length; i++)
                {
                    if (_currentlyLockedEnemy == null)
                    {
                        if (hit.collider == null)
                        {
                            //Hasn't hit anything, shoot ahead
                            _optionsGameObject[i].transform.right = new Vector3(transform.position.x + _noLockShotDistance, transform.position.y, 0) - _optionsGameObject[i].transform.position;
                        }
                        else
                        {
                            //Has hit, lock to enemy
                            _currentlyLockedEnemy = hit.transform.gameObject;
                            _optionsGameObject[i].transform.right = _currentlyLockedEnemy.transform.position - _optionsGameObject[i].transform.position;
                        }
                    }
                    else
                    {
                        //Point at locked target
                        _optionsGameObject[i].transform.right = _currentlyLockedEnemy.transform.position - _optionsGameObject[i].transform.position;
                    }

                    //Shoot
                    Instantiate(_secondaryShotPrefab,
                        _optionsGameObject[i].transform.position, _optionsGameObject[i].transform.rotation,
                        _playerBulletPool);
                }
            }
        }
    }

    private void ResetOptionPositions()
    {
        for (int i = 0; i < _optionsGameObject.Length; i++)
        {
            //TODOANIM make options move instead of teleport
            _optionsGameObject[i].transform.SetPositionAndRotation(_base1stOptionsLocation[i].transform.position, _base1stOptionsLocation[i].transform.rotation);
        }
    }

    private void RotateOptions()
    {
        _gunRotTime += Time.deltaTime;

        if(!_2ndShotEnabled)
        {
            _optionsGameObject[0].transform.position = Vector3.Lerp(_base1stOptionsLocation[1].position,
                _base1stOptionsLocation[0].position,
                (Mathf.Sin((_gunRotTime + 0.5f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset));

            _optionsGameObject[1].transform.position = Vector3.Lerp(_base1stOptionsLocation[0].position,
                _base1stOptionsLocation[1].position,
                (Mathf.Sin((_gunRotTime + 0.5f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset));
        }
        else
        {
            _optionsGameObject[0].transform.position = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                (Mathf.Sin((_gunRotTime + 0.25f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset));

            _optionsGameObject[1].transform.position = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                (Mathf.Sin((_gunRotTime + 0.5f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset));

            _optionsGameObject[2].transform.position = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                (Mathf.Sin((_gunRotTime + 0.75f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset));

            _optionsGameObject[3].transform.position = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                (Mathf.Sin((_gunRotTime + 1f) * _gunRotSpeed) * _gunRotAmplitude + _gunRotAmplitudeOffset));
        }
    }

    private void Block()
    {
        if (!_invincible && (_shieldRemainingCooldown == 0) && !_dead)
        {
            _shieldRemainingCooldown = _blockCooldown;
            _hitboxRenderer.color = _shieldOnCooldownColor;
            _shieldSprite.SetActive(true);
            _invincible = true;
            StartCoroutine(StopInvincibility(_blockDuration));
        }
    }

    private IEnumerator StopInvincibility(float time)
    {
        yield return new WaitForSeconds(time);

        if (time == _blockDuration)
        {
            _shieldSprite.SetActive(false);
        }
        _invincible = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Damage" && !_invincible)
        {
            GameProperties._life--;
            FindObjectOfType<LifeUI>().SetUpLifebar();

            transform.position = _playerRespawnLocation.position;

            _invincible = true;
            StartCoroutine(StopInvincibility(_deathTime));

            //TODOANIM Animate ship death
            HideShip();
            StartCoroutine(Respawn(_deathTime));

            _dead = true;
            CancelShooting();
            //TODOUI Use Credit screen
        }
    }

    private IEnumerator Respawn(float time)
    {
        yield return new WaitForSeconds(time);

        _dead = false;
        ShowShip();
    }

    private void HideShip()
    {
        foreach(var sprite in _playerSprites)
        {
            sprite.enabled = false;
        }
    }

    private void ShowShip()
    {
        foreach (var sprite in _playerSprites)
        {
            sprite.enabled = true;
        }
    }
}
