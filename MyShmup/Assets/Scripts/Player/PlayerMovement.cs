using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _fastSpeed;
    [SerializeField] private float _slowSpeed;
    private bool _slowMovement = true;
    private bool _fire = false;

    private Vector2 _movement;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private LayerMask _groundLayer;

    private PlayerInput playerInput;
    private InputAction fireAction;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
        fireAction = playerInput.actions["Fire"];
        fireAction.ReadValue<float>();
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

    public void OnMove(InputValue value) => _movement = value.Get<Vector2>();
}
