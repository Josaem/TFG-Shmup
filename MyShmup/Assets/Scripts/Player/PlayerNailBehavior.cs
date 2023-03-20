using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNailBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float _speed;
    public int _damage = 1;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.velocity = transform.right * _speed;
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.x) > 15 || Mathf.Abs(transform.position.y) > 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
        {
            enemy.TakeDamage(_damage);
            StickToEnemy();
            //update count
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void StickToEnemy()
    {
        GetComponent<PolygonCollider2D>().enabled = false;
        rb.velocity = transform.right * 0;
        rb.angularVelocity = 0;
    }
}
