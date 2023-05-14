using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    private float _laserSpeed;
    private float _maxDistance = 20;

    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private GameObject _laserVisualGuide;
    [SerializeField]
    private GameObject _laserStart;
    [SerializeField]
    private GameObject _laserEnd;

    private float _currentLaserDistance = 0;
    private bool _laserOn;

    public void Spawn(float speed, float distance, float guideTime)
    {
        _laserSpeed = speed;
        _maxDistance = distance;

        DontShowLaser();          

        //AnimateHere
        if (_laserVisualGuide != null && guideTime != 0)
        {
            _laserVisualGuide.SetActive(true);
            Invoke(nameof(ShowLaser), guideTime);
        }
        else
        {
            ShowLaser();
        }
    }

    private void ShowLaser()
    {
        _laserOn = true;

        if (_laserVisualGuide != null) _laserVisualGuide.SetActive(false);

        _laserStart.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void DontShowLaser()
    {
        _laserStart.SetActive(false);
        _laserEnd.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (_laserOn)
        {
            /*RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _maxDistance, _layerMask);

            if (hit.collider == null)
            {
                hit = Physics2D.Raycast(transform.TransformPoint(-Vector3.right * transform.localScale.x / 2), transform.up, _maxDistance, _layerMask);

                if (hit.collider == null)
                {
                    hit = Physics2D.Raycast(transform.TransformPoint(Vector3.right * transform.localScale.x / 2), transform.up, _maxDistance, _layerMask);

                    if (hit.collider == null)
                    {
                        transform.localScale = new Vector2(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 20, Time.deltaTime * _laserSpeed));
                        _currentLaserDistance = _maxDistance;
                        _laserEnd.SetActive(false);
                    }
                }
            }*/

            Vector2 raycastDirection = transform.TransformDirection(Vector2.up);

            RaycastHit2D hit = Physics2D.Raycast(transform.position, raycastDirection, _maxDistance, _layerMask);
            Debug.DrawRay(transform.position, raycastDirection * 40, Color.green);

            if (hit.collider == null)
            {
                Vector2 left = transform.position - transform.right * transform.localScale.x / 2;

                hit = Physics2D.Raycast(left, raycastDirection, _maxDistance, _layerMask);
                Debug.DrawRay(left, raycastDirection * 40, Color.green);

                if (hit.collider == null)
                {
                    Vector2 right = transform.position + transform.right * transform.localScale.x / 2;
                    hit = Physics2D.Raycast(right, raycastDirection, _maxDistance, _layerMask);
                    Debug.DrawRay(right, raycastDirection * 40, Color.green);

                    if (hit.collider == null)
                    {
                        transform.localScale = new Vector2(transform.localScale.x, Mathf.Lerp(transform.localScale.y, 20, Time.deltaTime * _laserSpeed));
                        _currentLaserDistance = _maxDistance;
                        _laserEnd.SetActive(false);
                    }
                }
            }

            if (hit.collider != null)
            {
                float hitDistance = hit.distance;
                if (_currentLaserDistance > hitDistance)
                {
                    _currentLaserDistance = hitDistance;
                    transform.localScale = new Vector2(transform.localScale.x, _currentLaserDistance);
                }
                else
                {
                    _currentLaserDistance = Mathf.Lerp(transform.localScale.y, hitDistance, Time.deltaTime * _laserSpeed);
                    transform.localScale = new Vector2(transform.localScale.x, _currentLaserDistance);
                }

                if (_currentLaserDistance >= hitDistance - 0.2)
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

    public void Kill()
    {
        _laserOn = false;
        //Animate here
        Destroy(transform.parent.gameObject);
    }
}
