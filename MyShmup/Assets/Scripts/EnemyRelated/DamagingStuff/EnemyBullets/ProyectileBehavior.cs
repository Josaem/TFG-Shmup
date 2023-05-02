using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProyectileBehavior : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected float _maxDistance = 20;
    protected float _speed = 5;
    [SerializeField]
    protected bool _traverseTerrain;
    [SerializeField]
    protected bool _traversePlayer;

    private Vector2 _originalPos;
    private bool _canDieFromBounds = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _originalPos = transform.position;
    }

    private void AllowBoundsDeath()
    {
        _canDieFromBounds = true;
    }

    protected virtual void Start(){ }

    protected virtual void Update()
    {
        if (!_canDieFromBounds && Mathf.Abs(transform.position.x) < 9 && Mathf.Abs(transform.position.y) < 5)
            AllowBoundsDeath();

        if (_canDieFromBounds && (Mathf.Abs(transform.position.x + transform.localScale.x / 2) > 10 || Mathf.Abs(transform.position.y + transform.localScale.y / 2) > 6))
        {
            Die();
        }
    }

    public virtual void Move(float speed)
    {
        _speed = speed;
        rb.velocity = transform.up * speed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt();
            if(!_traversePlayer)
                Destroy(gameObject);
        }
        else if(!_traverseTerrain)
        {
            Destroy(gameObject);
        }
    }

    public virtual void SetDeath(float maxDistance)
    {
        _maxDistance = maxDistance;
        float speed = _speed;
        if (speed == 0)
            speed = 1;
        float timeToDie = _maxDistance / speed;
        Invoke(nameof(Die), timeToDie);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
