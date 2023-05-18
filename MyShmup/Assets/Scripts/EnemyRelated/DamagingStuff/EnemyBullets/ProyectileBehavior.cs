using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProyectileBehavior : MonoBehaviour
{
    protected Rigidbody2D rb;
    [SerializeField]
    protected float _maxDistance = 20;
    protected float _speed = 5;
    [SerializeField]
    protected bool _traverseTerrain;
    [SerializeField]
    protected bool _traversePlayer;
    [SerializeField]
    protected LayerMask _whatTohit;

    private Vector2 _originalPos;
    private bool _canDieFromBounds = false;

    private float _initialScaleX = 0.1f;
    private float _targetScaleX = 1f;
    private float _initialScaleY = 0.1f;
    private float _targetScaleY = 1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _originalPos = transform.position;
    }

    private void AllowBoundsDeath()
    {
        _canDieFromBounds = true;
    }

    protected virtual void Start(){
        _targetScaleX = transform.localScale.x;
        _targetScaleY = transform.localScale.y;

        Vector3 initialScale = transform.localScale;
        initialScale.x = _initialScaleX;
        initialScale.y = _initialScaleY;
        transform.localScale = initialScale;

        StartCoroutine(GrowProjectile());
    }

    private IEnumerator GrowProjectile()
    {
        float growthRate = _speed * Time.deltaTime;

        while (transform.localScale.x < _targetScaleX && transform.localScale.y < _targetScaleY)
        {
            Vector3 currentScale = transform.localScale;
            currentScale.x += growthRate;
            currentScale.y += growthRate;
            transform.localScale = currentScale;
            yield return null;
        }
    }

    protected virtual void Update()
    {
        if (!_canDieFromBounds && Mathf.Abs(transform.position.x) < 9 && Mathf.Abs(transform.position.y) < 5)
            AllowBoundsDeath();

        if (_canDieFromBounds && ((Mathf.Abs(transform.position.x + transform.localScale.x / 2) > 10 || Mathf.Abs(transform.position.y + transform.localScale.y / 2) > 6)
            || (Mathf.Abs(transform.position.x + transform.localScale.x / 2) > 50 || Mathf.Abs(transform.position.y + transform.localScale.y / 2) > 40)))
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
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt();
            if (!_traversePlayer)
                Destroy(gameObject);
        }
        
        if (_whatTohit == (_whatTohit | (1 << collision.gameObject.layer)))
        {
            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == 7 && !_traverseTerrain)
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
        Animator bullet = GetComponentInChildren<Animator>();
        if (bullet != null)
        {
            bullet.Play("FadeBulletAnim");
            Destroy(gameObject, bullet.GetCurrentAnimatorStateInfo(0).length);
        }
        else Destroy(gameObject);
    }
}
