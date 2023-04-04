using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAttackBehavior : MonoBehaviour
{
    [SerializeField]
    private float _maxSize;
    [HideInInspector]
    public float _speed = 20;

    private void Update()
    {
        transform.localScale += new Vector3(Time.deltaTime * _speed, Time.deltaTime * _speed, 0);

        if (Mathf.Abs(transform.localScale.x) > _maxSize || Mathf.Abs(transform.localScale.y) > _maxSize)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null && Vector2.Distance(collision.transform.position, transform.position) > transform.localScale.x / 2 - 0.5)
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt();
        }
    }
}
