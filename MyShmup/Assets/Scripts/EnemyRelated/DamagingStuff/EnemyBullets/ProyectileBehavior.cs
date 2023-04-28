using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProyectileBehavior : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected float _maxDistance = 20;
    protected float _speed = 5;

    private Vector2 _originalPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _originalPos = transform.position;
    }

    protected virtual void Start(){ }

    protected virtual void Update(){ }

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
        }
        Destroy(gameObject);
    }

    public virtual void SetDeath(float maxDistance)
    {
        _maxDistance = maxDistance;
        float timeToDie = _maxDistance / _speed;
        Invoke(nameof(Die), timeToDie);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
