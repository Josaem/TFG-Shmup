using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProyectileBehavior : MonoBehaviour
{
    protected Rigidbody2D rb;
    public float _maxDistance = 20;
    public float _speed = 5;

    private Vector2 _originalPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _originalPos = transform.position;
    }

    protected virtual void Start()
    {
        SetDeath();
        Move();        
    }

    protected virtual void Update(){ }

    protected virtual void Move()
    {
        rb.velocity = transform.up * _speed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt();
        }
        Destroy(gameObject);
    }

    protected virtual void SetDeath()
    {
        float timeToDie = _maxDistance / _speed;
        Invoke(nameof(Die), timeToDie);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
