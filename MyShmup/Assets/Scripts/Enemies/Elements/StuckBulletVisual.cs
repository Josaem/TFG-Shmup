using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StuckBulletVisual : MonoBehaviour
{
    [SerializeField]
    private int _max1stShown = 50;
    [SerializeField]
    private int _max2ndShown = 30;
    [SerializeField]
    private float _timeToShowText = 1;
    [SerializeField]
    private SpriteRenderer _willDieFromExploVisual;
    [SerializeField]
    private GameObject _exploTextVisual;
    [SerializeField]
    private GameObject _primaryBulletVisual;
    [SerializeField]
    private GameObject _secondaryBulletVisual;

    private int _1stCounter;
    private int _2ndCounter;
    private TMP_Text _floatingText;
    private float _timeWhereTextActive = 0;
    private bool _textActive;
    private int _accumulatedScore = 0;
    private Transform _player;

    private void Start()
    {
        _floatingText = GetComponentInChildren<TMP_Text>();
        HideText();
        _willDieFromExploVisual.enabled = false;
        _player = FindObjectOfType<PlayerController>().transform;
    }

    private void Update()
    {
        if (_timeWhereTextActive > 0) _timeWhereTextActive = Mathf.Max(_timeWhereTextActive - Time.deltaTime, 0f);
        else if(_textActive)
        {
            _textActive = false;
            HideText();
        }

        if (_1stCounter > _max1stShown)
        {
            _1stCounter--;
            Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
        }

        if (_2ndCounter > _max2ndShown)
        {
            _2ndCounter--;
            Destroy(gameObject.transform.GetChild(1).GetChild(0).gameObject);
        }
    }

    public void GotHit(bool isPrimary, Vector3 bulletPos, int accumulatedScore)
    {
        _accumulatedScore = accumulatedScore;

        Vector3 relativePos = transform.position - _player.transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

        if (isPrimary)
        {
            _1stCounter++;
            Instantiate(_primaryBulletVisual, bulletPos, Quaternion.AngleAxis(angle, Vector3.forward), transform.GetChild(0));
        }
        else
        {
            _2ndCounter++;
            Instantiate(_secondaryBulletVisual, bulletPos, Quaternion.AngleAxis(angle, Vector3.forward), transform.GetChild(1));
        }

        ShowText();
    }

    private void ShowText()
    {
        _textActive = true;
        _timeWhereTextActive = _timeToShowText;
        _floatingText.text = _accumulatedScore.ToString();
    }

    private void HideText()
    {
        _floatingText.text = "";
    }

    public void GotExploded(bool gotKilled, int enemiesDead, int accumulatedScore)
    {
        if(!gotKilled)
        {
            foreach(Transform child in transform.GetChild(0))
            {
                Destroy(child.gameObject);
            }

            _accumulatedScore = 0;
            ShowText();
        }
        else
        {
            ExplosionText visual = Instantiate(_exploTextVisual, transform.position, Quaternion.identity).GetComponent<ExplosionText>();
            visual._enemiesKilled = enemiesDead;
            visual._score = accumulatedScore;

            Vector2 explosionPos = transform.position;
            bool outOfBounds = false;

            if(transform.position.x < - 7)
            {
                explosionPos.x = - 5;
                outOfBounds = true;
            }

            if (transform.position.x > 7)
            {
                explosionPos.x = 5;
                outOfBounds = true;
            }

            if (transform.position.y > 4)
            {
                explosionPos.y = 4;
                outOfBounds = true;
            }

            if (transform.position.y < -4)
            {
                explosionPos.y = -4;
                outOfBounds = true;
            }

            visual.transform.position = explosionPos;

            visual.Spawn(outOfBounds);
        }
    }

    public void EnemyWillDieByExplosion()
    {
        _willDieFromExploVisual.enabled = true;
    }

    /*
     * Si matas con explosion, muestra texto con explosion y lo deja en pantalla poco tiempo -> spawnear un prefab que se autodestruye a los pocos segundos
     * 
     */
}
