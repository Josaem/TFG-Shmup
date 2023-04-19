using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBehavior : MonoBehaviour
{
    public float _laserSpeed;
    public float _maxDistance = 20;
    public float _timeToSpawn = 0.5f;

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

    private void Start()
    {
        DontShowLaser();

        //AnimateHere
        Invoke(nameof(Spawn), _timeToSpawn);
    }

    private void Spawn()
    {
        ShowLaser();
        _laserOn = true;
    }

    private void ShowLaser()
    {
        _laserVisualGuide.SetActive(false);
        _laserStart.SetActive(true);
        GetComponent<Collider2D>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void DontShowLaser()
    {
        _laserStart.SetActive(false);
        _laserVisualGuide.SetActive(true);
        _laserEnd.SetActive(false);
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(false);
    }

    private void Update()
    {
        if(_laserOn)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _maxDistance, _layerMask);

            if (hit.collider == null)
            {
                hit = Physics2D.Raycast(transform.TransformPoint(-Vector3.right * transform.localScale.x / 2), transform.up, _maxDistance, _layerMask);

                if (hit.collider == null)
                {
                    hit = Physics2D.Raycast(transform.TransformPoint(Vector3.right * transform.localScale.x / 2), transform.up, _maxDistance, _layerMask);

                    if (hit.collider == null)
                    {
                        transform.localScale = new Vector2(1, Mathf.Lerp(transform.localScale.y, 20, Time.deltaTime * _laserSpeed));
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
                    transform.localScale = new Vector2(1, _currentLaserDistance);
                }
                else
                {
                    _currentLaserDistance = Mathf.Lerp(transform.localScale.y, hitDistance, Time.deltaTime * _laserSpeed);
                    transform.localScale = new Vector2(1, _currentLaserDistance);
                }

                if (_currentLaserDistance >= hitDistance - 0.1)
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
        Destroy(gameObject);
    }
}
