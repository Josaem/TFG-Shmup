using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisFollowShootingEnemy : ShootingEnemy
{
    [SerializeField]
    private bool _horizontal;
    [SerializeField]
    private float _speed;
    [SerializeField]
    private float _maxFollow;

    private Vector2 _ogPos;

    public override void StartAction()
    {
        base.StartAction();
        _ogPos = transform.position;
    }

    public override void Move()
    {
        Transform playerPos = GameObject.FindObjectOfType<PlayerController>().transform;

        if (_horizontal)
        {
            if(transform.position.x < playerPos.transform.position.x)
            {
                if(transform.position.x < _ogPos.x + _maxFollow)
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerPos.position.x, transform.position.y),
                        _speed * Time.deltaTime);
            }
            else if(transform.position.x > _ogPos.x - _maxFollow)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerPos.position.x, transform.position.y),
                    _speed * Time.deltaTime);
            }
        }
        else
        {
            if (transform.position.y < playerPos.transform.position.y)
            {
                if (transform.position.y < _ogPos.y + _maxFollow)
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, playerPos.position.y),
                        _speed * Time.deltaTime);
            }
            else if (transform.position.y > _ogPos.y - _maxFollow)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, playerPos.position.y),
                    _speed * Time.deltaTime);
            }
        }
    }
}
