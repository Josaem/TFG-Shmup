using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    public float _shootSpeed;
    public float _maxDistance = 20;
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private GameObject _laserEnd;
    private float _currentLaserDistance = 0;

    private void Start()
    {
        _laserEnd.SetActive(false);
    }
    private void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _maxDistance, _layerMask);

        if(hit.collider == null)
        {
            hit = Physics2D.Raycast(transform.TransformPoint(-Vector3.right * transform.localScale.x/2), transform.up, _maxDistance, _layerMask);

            if(hit.collider == null)
            {
                hit = Physics2D.Raycast(transform.TransformPoint(Vector3.right * transform.localScale.x / 2), transform.up, _maxDistance, _layerMask);

                if(hit.collider == null)
                {
                    transform.localScale = new Vector2(1, Mathf.Lerp(transform.localScale.y, 20, Time.deltaTime * _shootSpeed));
                    _currentLaserDistance = _maxDistance;
                    _laserEnd.SetActive(false);
                }
            }
        }
        
        if(hit.collider != null)
        {
            float hitDistance = hit.distance;
            if (_currentLaserDistance > hitDistance)
            {
                _currentLaserDistance = hitDistance;
                transform.localScale = new Vector2(1, _currentLaserDistance);
            }
            else
            {
                _currentLaserDistance = Mathf.Lerp(transform.localScale.y, hitDistance, Time.deltaTime * _shootSpeed);
                transform.localScale = new Vector2(1, _currentLaserDistance);
            }
            
            if(_currentLaserDistance >= hitDistance -0.1)
            {
                _laserEnd.SetActive(true);
                _laserEnd.transform.localPosition = new Vector2(0, hitDistance);
            }
            else
            {
                _laserEnd.SetActive(false);
            }
        }
    }
}