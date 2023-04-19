using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieByPos : MonoBehaviour
{
    [SerializeField]
    private MoreOrLess _xSelector;
    [SerializeField]
    private float _xDeath;
    [SerializeField]
    private MoreOrLess _ySelector;
    [SerializeField]
    private float _yDeath;

    private bool _deadX;
    private bool _deadY;

    private enum MoreOrLess
    {
        none,
        moreThan,
        lessThan
    }

    private void Update()
    {
        if (_xSelector != MoreOrLess.none)
        {
            if (_xSelector == MoreOrLess.moreThan && transform.position.x > _xDeath)
            {
                _deadX = true;
            }

            if (_xSelector == MoreOrLess.lessThan && transform.position.x < _xDeath)
            {
                _deadX = true;
            }
        }
        else _deadX = true;

        if (_ySelector != MoreOrLess.none)
        {
            if (_ySelector == MoreOrLess.moreThan && transform.position.y > _yDeath)
            {
                _deadY = true;
            }

            if (_ySelector == MoreOrLess.lessThan && transform.position.y < _yDeath)
            {
                _deadY = true;
            }
        }
        else _deadY = true;

        if (_deadX && _deadY)
        {
            Kill();
        }
    }

    private void Kill()
    {
        if(TryGetComponent<Enemy>(out Enemy enemy))
        {
            enemy.DieByWaypoint();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
