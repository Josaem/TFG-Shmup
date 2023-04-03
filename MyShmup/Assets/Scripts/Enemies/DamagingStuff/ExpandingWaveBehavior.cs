using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandingWaveBehavior : MonoBehaviour
{
    [HideInInspector]
    public float _speed = 20;

    private void Update()
    {
        transform.localScale += new Vector3(Time.deltaTime * _speed, Time.deltaTime * _speed, 0);

        if (Mathf.Abs(transform.localScale.x) > 30 || Mathf.Abs(transform.localScale.y) > 30)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null && Vector2.Distance(collision.transform.position, transform.position) > transform.localScale.x / 2 - 0.25)
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt();
        }
    }
}
