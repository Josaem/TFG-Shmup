using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float _speed;
    [SerializeField] private float _maxDistance = 20;
    public int _damage = 1;
    [SerializeField] private bool _isPrimary;
    [SerializeField] private GameObject _hitVisual;

    private bool _isDead = false;
    private Vector2 _originalPos;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _originalPos = transform.position;
        rb.velocity = transform.right * _speed;
    }

    private void Update()
    {
        if (Vector2.Distance(_originalPos, transform.position) > _maxDistance)
        {
            Die();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(!_isDead)
        {
            if (collision.gameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(_damage, _isPrimary, collision.ClosestPoint(transform.position));
            }
            rb.angularVelocity = 0;
            _isDead = true;
            Instantiate(_hitVisual, collision.ClosestPoint(transform.position), transform.rotation);
            Destroy(gameObject);
        } 
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
