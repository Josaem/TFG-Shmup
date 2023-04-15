using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    public float _maxDistance = 20;
    [HideInInspector]
    public float _speed = 20;

    private Vector2 _originalPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _originalPos = transform.position;
    }

    private void Start()
    {
        rb.velocity = transform.up.normalized * _speed;
    }

    private void Update()
    {
        if (Vector2.Distance(_originalPos, transform.position) > _maxDistance)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt();
        }
        Destroy(gameObject);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
