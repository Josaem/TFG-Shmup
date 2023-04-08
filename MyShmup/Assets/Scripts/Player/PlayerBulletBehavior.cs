using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float _speed;
    public int _damage = 1;
    [SerializeField] private bool _isPrimary;

    private bool _isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.right * _speed;
    }

    private void Update()
    {
        if (Mathf.Abs(transform.position.x) > 15 || Mathf.Abs(transform.position.y) > 10)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if(!_isDead)
        {
            if (collision.gameObject.TryGetComponent<Enemy>(out var enemy))
            {
                enemy.TakeDamage(_damage, _isPrimary, collision.contacts[0].point, transform.rotation);
            }
            rb.angularVelocity = 0;
            _isDead = true;
            Destroy(gameObject);
        } 
    }
}
