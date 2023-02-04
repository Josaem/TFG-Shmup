using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _fastSpeed;
    [SerializeField] private float _slowSpeed;
    private bool _slowMovement = false;
    private bool _invincible = false;
    [SerializeField] private float _blockTime;
    [SerializeField] private float _blockCoolDown;
    [SerializeField] private float _shieldRemainingCooldown = 0;
    [SerializeField] private float _deathTime;
    [SerializeField] private Color _shieldOnCooldownColor;
    [SerializeField] private Color _shieldNotOnCooldownColor;
    private bool _dead = false;

    private Vector2 _movement;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private LayerMask _groundLayer;

    private PlayerInput playerInput;
    private InputAction fireAction;
    private InputAction blockAction;

    private SpriteRenderer[] _playerSprites;

    [SerializeField] private GameObject _shieldSprite;
    [SerializeField] private GameObject _hitboxSprite;
    private SpriteRenderer _hitboxRenderer;
    [SerializeField] private Transform _playerRespawnLocation;

    [SerializeField] private bool _1stShotEnabled = false;
    [SerializeField] private float _fireTime;
    private float _fireRemainingCooldown;
    [SerializeField] private float _fireRate;
    private float _timeToShoot = 0;
    private bool _fireButtonPressed = false;
    [SerializeField] private GameObject _playerShotVulcan;

    [Header("SecondaryShot")]
    [SerializeField]  private bool _2ndShotEnabled = false;
    [SerializeField] private float _timeTo2ndShot;
    private float _timeUntil2ndShot;
    [SerializeField] private GameObject _secondaryShot;
    [SerializeField] private Transform[] _base2ndOptionsLocation;

    [SerializeField] private Transform[] _base1stOptionsLocation;
    [SerializeField] private Transform _playerBulletPool;

    [SerializeField] private GameObject[] _currentOptionsLocation;


    public float speed = 5f;
    public float amplitude = 0.5f;
    public float amplitudeOffset = 0.5f;

    [SerializeField] private float _gunRotTimer = 0;

    [SerializeField] private LayerMask _enemyLayerMask;

    private GameObject _currentEnemy;

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
        _timeUntil2ndShot = _timeTo2ndShot;
        ResetGunPositions();

        fireAction.started += ctx => {
            StartShooting();
        };
        fireAction.canceled += ctx => {
            CancelShooting();
        };

        blockAction.performed += ctx => {
            Block();
        };
    }

    private void Update()
    {
        if(!_dead)
        {
            //Decrease shield cooldown
            if (_shieldRemainingCooldown > 0) _shieldRemainingCooldown = Mathf.Max(_shieldRemainingCooldown - Time.deltaTime, 0f);

            //Change hitbox color depending on shield cooldown
            if (_shieldRemainingCooldown == 0 && _hitboxRenderer.color != _shieldNotOnCooldownColor) _hitboxRenderer.color = _shieldNotOnCooldownColor;

            //Decrease time where fire is active 
            if (_fireRemainingCooldown > 0) _fireRemainingCooldown = Mathf.Max(_fireRemainingCooldown - Time.deltaTime, 0f);
            if (_fireRemainingCooldown > 0 && _timeUntil2ndShot > 0) Fire();

            //Decrease time where fire is active 
            if (_timeUntil2ndShot > 0 && _fireButtonPressed) _timeUntil2ndShot = Mathf.Max(_timeUntil2ndShot - Time.deltaTime, 0f);
            if (_timeUntil2ndShot == 0 && _fireButtonPressed) Fire2nd();

            if (_fireRemainingCooldown == 0 && _1stShotEnabled)
                _1stShotEnabled = false;

            //If not shooting
            if (!_1stShotEnabled && !_2ndShotEnabled)
            {
                ResetGunPositions();
                _gunRotTimer = 0;
            }
            else
            {
                RotateGuns();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_dead)
        {
            Move();
        }
    }

    private void Move()
    {
        if (_slowMovement)
            _rb.velocity = _movement * _slowSpeed * Time.deltaTime;
        else
            _rb.velocity = _movement * _fastSpeed * Time.deltaTime;
    }

    private void StartShooting()
    {
        _fireRemainingCooldown = _fireTime;
        _fireButtonPressed = true;
        _timeUntil2ndShot = _timeTo2ndShot;
        _1stShotEnabled = true;
    }

    private void Fire()
    {
        _1stShotEnabled = true;
        if(Time.time > _timeToShoot)
        {
            _timeToShoot = Time.time + _fireRate;

            for (int i = 0; i < _currentOptionsLocation.Length; i++) //GameObject gun in _currentOptionsLocation)
            {
                //TODO Change to 2nd shot, set rotation to face the raycast hit
                Instantiate(_playerShotVulcan,
                    _currentOptionsLocation[i].transform.position, _currentOptionsLocation[i].transform.rotation,
                    _playerBulletPool);
            }
        }
    }

    private void Fire2nd()
    {
        if(_slowMovement == false) _slowMovement = true;
        _2ndShotEnabled = true;

        if (Time.time > _timeToShoot)
        {
            _timeToShoot = Time.time + _fireRate;

            for(int i = 0; i < _currentOptionsLocation.Length; i++)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, 15, _enemyLayerMask);

                if (_currentEnemy == null)
                {
                    if (hit.collider == null)
                    {
                        _currentOptionsLocation[i].transform.right = new Vector3(transform.position.x + 10, transform.position.y, 0) - _currentOptionsLocation[i].transform.position;
                    }
                    else if(hit.distance < 3)
                    {
                        _currentOptionsLocation[i].transform.right = new Vector3(transform.position.x + 3, transform.position.y, 0) - _currentOptionsLocation[i].transform.position;
                    }
                    else
                    {
                        _currentEnemy = hit.transform.gameObject;
                    }
                }
                else
                {
                    _currentOptionsLocation[i].transform.right = _currentEnemy.transform.position - _currentOptionsLocation[i].transform.position;
                }
                
                Instantiate(_playerShotVulcan,
                    _currentOptionsLocation[i].transform.position, _currentOptionsLocation[i].transform.rotation,
                    _playerBulletPool);
            }
        }
    }

    private void CancelShooting()
    {
        _slowMovement = false;
        _fireButtonPressed = false;
        _2ndShotEnabled = false;
        _currentEnemy = null;
    }

    private void ResetGunPositions()
    {
        for (int i = 0; i < _currentOptionsLocation.Length; i++) //GameObject gun in _currentOptionsLocation)
        {
            _currentOptionsLocation[i].transform.position = _base1stOptionsLocation[i].transform.position;
            _currentOptionsLocation[i].transform.rotation = _base1stOptionsLocation[i].transform.rotation;
        }
    }

    private void RotateGuns()
    {
        _gunRotTimer += Time.deltaTime;
        if(!_2ndShotEnabled)
        {
            _currentOptionsLocation[0].transform.position = Vector3.Lerp(_base1stOptionsLocation[1].position,
                _base1stOptionsLocation[0].position,
                (Mathf.Sin((_gunRotTimer + 0.5f) * speed) * amplitude + amplitudeOffset));

            _currentOptionsLocation[1].transform.position = Vector3.Lerp(_base1stOptionsLocation[0].position,
                _base1stOptionsLocation[1].position,
                (Mathf.Sin((_gunRotTimer + 0.5f) * speed) * amplitude + amplitudeOffset));
        }
        else
        {
            _currentOptionsLocation[0].transform.position = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                (Mathf.Sin((_gunRotTimer + 0.25f) * speed) * amplitude + amplitudeOffset));

            _currentOptionsLocation[1].transform.position = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                (Mathf.Sin((_gunRotTimer + 0.5f) * speed) * amplitude + amplitudeOffset));

            _currentOptionsLocation[2].transform.position = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                (Mathf.Sin((_gunRotTimer + 0.75f) * speed) * amplitude + amplitudeOffset));

            _currentOptionsLocation[3].transform.position = Vector3.Lerp(_base2ndOptionsLocation[0].position,
                _base2ndOptionsLocation[2].position,
                (Mathf.Sin((_gunRotTimer + 1f) * speed) * amplitude + amplitudeOffset));
        }
    }

    private void Block()
    {
        if (!_invincible && (_shieldRemainingCooldown == 0) && !_dead)
        {
            _shieldRemainingCooldown = _blockCoolDown;
            _hitboxRenderer.color = _shieldOnCooldownColor;
            _shieldSprite.SetActive(true);
            _invincible = true;
            StartCoroutine(StopInvincibility(_blockTime));
        }
    }

    public void OnMove(InputValue value) => _movement = value.Get<Vector2>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Damage" && !_invincible)
        {
            GameProperties._life--;
            FindObjectOfType<LifeUI>().SetUpLifebar();

            transform.position = _playerRespawnLocation.position;

            _invincible = true;
            StartCoroutine(StopInvincibility(_deathTime));

            //Animate ship death
            HideShip();
            StartCoroutine(Respawn(_deathTime));

            if(GameProperties._life <= 0)
            {
                //TODO Use Credit screen
                _dead = true;
            }
        }
    }

    private IEnumerator StopInvincibility(float time)
    {
        yield return new WaitForSeconds(time);

        if(time == _blockTime)
        {
            _shieldSprite.SetActive(false);
        }
        _invincible = false;
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
