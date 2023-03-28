using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisFollowShootingEnemy : ShootingEnemy
{
    [SerializeField]
    private bool _horizontal;
    [SerializeField]
    private float _speed;

    public override void Move()
    {
        Transform playerPos = GameObject.FindObjectOfType<PlayerController>().transform;

        if (_horizontal)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerPos.position.x, transform.position.y),
                _speed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, playerPos.position.y),
                _speed * Time.deltaTime);
        }
    }
}
