using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    [HideInInspector]
    public float _speed = 20;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rb.velocity = transform.up * _speed;
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
        Destroy(gameObject);
    }
}
