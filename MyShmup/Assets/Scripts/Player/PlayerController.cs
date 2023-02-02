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
    private bool _fire = false;
    private bool _invincible = false;
    [SerializeField] private float _blockTime;
    [SerializeField] private float _blockCoolDown;
    [SerializeField] private float _shieldRemainingCooldown = 0;
    [SerializeField] private float _deathTime;
    [SerializeField] private Color _shieldOnCooldownColor;
    [SerializeField] private Color _shieldNotOnCooldownColor;

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
        fireAction.performed += ctx => {
            _fire = true;
            _slowMovement = true;
        };
        fireAction.canceled += ctx => { _fire = false;
            _slowMovement = false;
        };

        blockAction.performed += ctx => {
            Block();
        };
    }

    private void Update()
    {
        if(_shieldRemainingCooldown > 0) _shieldRemainingCooldown = Mathf.Max(_shieldRemainingCooldown - Time.deltaTime, 0f);
        if (_shieldRemainingCooldown == 0 && _hitboxRenderer.color != _shieldNotOnCooldownColor) _hitboxRenderer.color = _shieldNotOnCooldownColor;
    }

    private void FixedUpdate()
    {
        Move();

        if(_fire) Fire();
    }

    private void Move()
    {
        if (_slowMovement)
            _rb.velocity = _movement * _slowSpeed * Time.deltaTime;
        else
            _rb.velocity = _movement * _fastSpeed * Time.deltaTime;
    }

    private void Fire()
    {

    }

    private void Block()
    {
        if (!_invincible && (_shieldRemainingCooldown == 0))
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
            StartCoroutine(StopHiding(_deathTime));

            if(GameProperties._life <= 0)
            {
                //TODO Use Credit screen
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

    private IEnumerator StopHiding(float time)
    {
        yield return new WaitForSeconds(time);

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
