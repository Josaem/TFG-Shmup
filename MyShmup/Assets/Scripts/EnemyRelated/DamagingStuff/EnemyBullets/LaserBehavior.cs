using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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

    private Collider2D _myCollider;
    private int _numObjectsInsideTrigger;
    private float _targetDistance;
    private bool _laserOn;
    private float _laserEndBasePos;

    public void Spawn(float speed, float distance, float guideTime)
    {
        _myCollider = GetComponent<Collider2D>();
        _laserSpeed = speed;
        _maxDistance = distance;
        _targetDistance = _maxDistance;
        _laserEndBasePos = transform.localPosition.y;

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
        _laserEnd.SetActive(true);
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
            Vector3 newScale = transform.lossyScale;
            if (transform.lossyScale.y < _targetDistance)
            {
                newScale.y += _laserSpeed * Time.deltaTime;
                transform.localScale = newScale;
            }
            else
            {
                newScale.y = _targetDistance;
                transform.localScale = newScale;
            }

            _laserEnd.transform.localPosition = new Vector2(0, transform.localScale.y + _laserEndBasePos);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & _layerMask) != 0)
        {
            // Increment the count when an object enters the trigger
            _numObjectsInsideTrigger++;

            Vector2 contactPoint = other.ClosestPoint(transform.position);

            Vector2 localContactPoint = transform.InverseTransformPoint(contactPoint);

            Vector3 toObject = contactPoint - (Vector2)transform.position;
            float yPosition = Vector3.Dot(toObject, transform.up);

            float contactDistance = yPosition; //localContactPoint.y;

            if (contactDistance < _targetDistance)
                _targetDistance = Mathf.Min(_targetDistance, contactDistance);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & _layerMask) != 0)
        {
            Vector2 contactPoint = other.ClosestPoint(transform.position);

            Vector2 localContactPoint = transform.InverseTransformPoint(contactPoint);

            Vector3 toObject = contactPoint - (Vector2)transform.position;
            float yPosition = Vector3.Dot(toObject, transform.up);

            float contactDistance = yPosition; // localContactPoint.y;

            if (contactDistance < _targetDistance)
                _targetDistance = Mathf.Min(_targetDistance, contactDistance);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (((1 << other.gameObject.layer) & _layerMask) != 0)
        {
            // Decrement the count when an object exits the trigger
            _numObjectsInsideTrigger--;
            if (_numObjectsInsideTrigger == 0)
            {
                _targetDistance = _maxDistance;
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
