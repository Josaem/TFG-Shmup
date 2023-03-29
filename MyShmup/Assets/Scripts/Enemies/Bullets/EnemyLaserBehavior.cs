using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLaserBehavior : MonoBehaviour
{
    public float _shootSpeed;
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private GameObject _laserEnd;

    private void Start()
    {
        _laserEnd.SetActive(false);
    }
    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, 20, _layerMask);

        if(hit.collider == null)
        {
            hit = Physics2D.Raycast(transform.TransformPoint(-Vector3.right * transform.localScale.x/2), transform.up, 20, _layerMask);

            if(hit.collider == null)
            {
                hit = Physics2D.Raycast(transform.TransformPoint(Vector3.right * transform.localScale.x / 2), transform.up, 20, _layerMask);

                if(hit.collider == null)
                {
                    transform.localScale = new Vector2(1, Mathf.Lerp(transform.localScale.y, 20, Time.time * _shootSpeed));
                    _laserEnd.SetActive(false);
                }
            }
        }
        
        if(hit.collider != null)
        {
            transform.localScale = new Vector2(1, Mathf.Lerp(transform.localScale.y, Vector2.Distance(transform.position, hit.collider.transform.position)-0.5f, Time.time * _shootSpeed));
            _laserEnd.SetActive(true);
            _laserEnd.transform.localPosition = new Vector2(0, Vector2.Distance(transform.position, hit.collider.transform.position) - 0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>() != null)
        {
            collision.gameObject.GetComponent<PlayerController>().GetHurt();
        }
    }
}
