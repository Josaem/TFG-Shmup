using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StuckBulletVisual : MonoBehaviour
{
    [SerializeField]
    private int _maxBulletsShown;
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

    private int _bulletCounter;
    private TMP_Text _floatingText;
    private float _timeWhereTextActive = 0;
    private bool _textActive;
    private int _accumulatedScore = 0;

    private void Start()
    {
        _floatingText = GetComponentInChildren<TMP_Text>();
        HideText();
        _willDieFromExploVisual.enabled = false;
    }

    private void Update()
    {
        if (_timeWhereTextActive > 0) _timeWhereTextActive = Mathf.Max(_timeWhereTextActive - Time.deltaTime, 0f);
        else if(_textActive)
        {
            _textActive = false;
            HideText();
        }

        if (_bulletCounter > _maxBulletsShown)
        {
            _bulletCounter--;
            Destroy(gameObject.transform.GetChild(0).GetChild(0).gameObject);
        }
    }

    public void GotHit(bool isPrimary, Vector3 bulletPos, Quaternion bulletRot, int accumulatedScore)
    {
        _bulletCounter++;

        _accumulatedScore = accumulatedScore;

        if(isPrimary)
        {
            Instantiate(_primaryBulletVisual, bulletPos, bulletRot, transform.GetChild(0));
        }
        else
        {
            Instantiate(_secondaryBulletVisual, bulletPos, bulletRot, transform.GetChild(0));
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
            visual.Spawn();
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
