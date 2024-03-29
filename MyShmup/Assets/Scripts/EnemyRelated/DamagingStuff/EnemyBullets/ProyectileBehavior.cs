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
    private float _timeAlive;

    private Collider2D _myCollider2D;

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
        if(_speed == 0)
        {
            _speed = 5f;
        }

        _targetScaleX = transform.localScale.x;
        _targetScaleY = transform.localScale.y;

        Vector3 initialScale = transform.localScale;
        initialScale.x = _initialScaleX;
        initialScale.y = _initialScaleY;
        transform.localScale = initialScale;

        StartCoroutine(GrowProjectile());

        _myCollider2D = GetComponent<Collider2D>();
        if (_myCollider2D != null)
            IgnoreSpawnerCollision();
    }

    private IEnumerator GrowProjectile()
    {
        while (transform.localScale.x < _targetScaleX && transform.localScale.y < _targetScaleY)
        {
            float growthRate = _speed * Time.deltaTime;
            Vector3 currentScale = transform.localScale;
            currentScale.x += growthRate;
            currentScale.y += growthRate;
            if(currentScale.x > _targetScaleX)
            {
                currentScale.x = _targetScaleX;
            }

            if(currentScale.y > _targetScaleY)
            {
                currentScale.y = _targetScaleY;
            }
            
            transform.localScale = currentScale;
            yield return null;
        }       
    }

    private void IgnoreSpawnerCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.5f);

        foreach (Collider2D collider in colliders)
        {
            if (collider !=_myCollider2D)
            {
                Physics2D.IgnoreCollision(_myCollider2D, collider);
            }
        }
    }

    protected virtual void Update()
    {
        _timeAlive += Time.deltaTime;
        if (!_canDieFromBounds && HasEnteredScreen())
            AllowBoundsDeath();

        if (_canDieFromBounds && IsOffscreen())
        {
            Die();
        }

        if (_timeAlive >= 15)
            AllowBoundsDeath();
    }

    private bool HasEnteredScreen()
    {
        if (transform.position.x < 9 && transform.position.x > -9
            && transform.position.y < 5 && transform.position.y > -5)
            return true;
        return false;
    }

    private bool IsOffscreen()
    {
        if (transform.position.x > 13 || transform.position.x < -13 ||
            transform.position.y > 8 || transform.position.y < -8)
            return true;
        return false;
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
            else if (((1 << collision.gameObject.layer) & _whatTohit) != 0)
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
        if (_maxDistance != 0)
        {
            _maxDistance = maxDistance;
            float speed = _speed;
            if (speed == 0)
                speed = 1;
            float timeToDie = _maxDistance / speed;
            Invoke(nameof(Die), timeToDie);
        }
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
