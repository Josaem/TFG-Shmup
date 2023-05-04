using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAttackBehavior : MonoBehaviour
{
    public float _maxSize = 30;
    public float _speed = 20;
    public float _cooldownDivider = 3;
    [HideInInspector]
    public Transform _followTarget;

    private float _growthTime = 0;
    private bool _spawned = false;

    private void Start()
    {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    private void FixedUpdate()
    {
        if(!_spawned)
        {
            GetComponentInChildren<SpriteRenderer>().enabled = true;
            _spawned = true;
        }

        _growthTime += Time.deltaTime;

        if(_followTarget != null )
            transform.position = _followTarget.transform.position;

        transform.localScale = new Vector3( _growthTime * _speed, _growthTime * _speed, 1);

        if (Mathf.Abs(transform.localScale.x) > _maxSize || Mathf.Abs(transform.localScale.y) > _maxSize)
        {
            Die();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null && Vector2.Distance(collision.transform.position, transform.position) > transform.localScale.x - 0.5)
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt();
            collision.gameObject.GetComponent<PlayerController>().ResetShield(_cooldownDivider);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
