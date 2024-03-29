using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StuckBulletVisual : MonoBehaviour
{
    [SerializeField]
    private float _textDistanceFromObject = 1;
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
    private Transform _enemyTransform;

    private GameObject _stuck1stPool;
    private GameObject _stuck2ndPool;

    private void Start()
    {
        _enemyTransform = transform.parent;
        _floatingText = GetComponentInChildren<TMP_Text>();
        HideText();
        _willDieFromExploVisual.enabled = false;
        _player = FindObjectOfType<PlayerController>().transform;

        _stuck1stPool = new GameObject("Stuck1stPool");
        _stuck2ndPool = new GameObject("Stuck2ndPool");

        _stuck1stPool.transform.SetParent(transform);
        _stuck2ndPool.transform.SetParent(transform);
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

            Animator bullet = _stuck1stPool.GetComponentInChildren<Animator>();
            bullet.Play("DestroyStuckBulletAnim");
            Destroy(bullet.gameObject, bullet.GetCurrentAnimatorStateInfo(0).length);
        }

        if (_2ndCounter > _max2ndShown)
        {
            _2ndCounter--;

            Animator bullet = _stuck2ndPool.GetComponentInChildren<Animator>();
            bullet.Play("DestroyStuckBulletAnim");
            Destroy(bullet.gameObject, bullet.GetCurrentAnimatorStateInfo(0).length);
        }
    }

    private void FixedUpdate()
    {
        _willDieFromExploVisual.transform.rotation = Quaternion.identity;
    }

    public void GotHit(bool isPrimary, Vector3 bulletPos, int accumulatedScore)
    {
        _accumulatedScore = accumulatedScore;

        Vector3 relativePos = transform.position - _player.transform.position;
        float angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;

        Vector3 inverseScale = new (1f / _enemyTransform.localScale.x, 1f / _enemyTransform.localScale.y, 1f);

        SpriteRenderer bullet;

        if (isPrimary)
        {
            _1stCounter++;
            bullet = Instantiate(_primaryBulletVisual, bulletPos, Quaternion.AngleAxis(angle, Vector3.forward), _stuck1stPool.transform).GetComponentInChildren<SpriteRenderer>();
        }
        else
        {
            _2ndCounter++;
            bullet = Instantiate(_secondaryBulletVisual, bulletPos, Quaternion.AngleAxis(angle, Vector3.forward), _stuck2ndPool.transform).GetComponentInChildren<SpriteRenderer>();
        }

        bullet.transform.localScale = Vector3.Scale(bullet.transform.localScale, inverseScale);
        bullet.sortingOrder = Random.Range(0, 100);

        ShowText();
    }

    private void ShowText()
    {
        _textActive = true;
        _timeWhereTextActive = _timeToShowText;
        _floatingText.text = _accumulatedScore.ToString();

        if(transform.position.y > 3.6) _floatingText.transform.position = new Vector2(transform.position.x, transform.position.y - _textDistanceFromObject / 2);
        else _floatingText.transform.position = new Vector2(transform.position.x, transform.position.y + _textDistanceFromObject/2 + 0.5f);
        _floatingText.transform.rotation = Quaternion.identity;
    }

    private void HideText()
    {
        _floatingText.text = "";
    }

    public void GotExploded(bool gotKilled, int enemiesDead, int accumulatedScore)
    {
        _willDieFromExploVisual.enabled = false;

        foreach (Animator child in _stuck1stPool.GetComponentsInChildren<Animator>())
        {
            child.Play("KillWithStuckBulletAnim");
            Destroy(child.gameObject, child.GetCurrentAnimatorStateInfo(0).length);
        }

        foreach (Animator child in _stuck2ndPool.GetComponentsInChildren<Animator>())
        {
            child.Play("KillWithStuckBulletAnim");
            Destroy(child.gameObject, child.GetCurrentAnimatorStateInfo(0).length);
        }

        _1stCounter = 0;
        _2ndCounter = 0;
        _accumulatedScore = 0;
        ShowText();

        if (gotKilled)
        {
            //TODO Spawn explosion
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
        _willDieFromExploVisual.GetComponent<Animator>().Play("WillDieFromExploEntryAnim");
    }

    private void OnDestroy()
    {
        Destroy(_stuck1stPool);
        Destroy(_stuck2ndPool);
    }

    public float TimeToDieByExplo()
    {
        if(_stuck1stPool.transform.childCount > 0)
        {
            return _stuck1stPool.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length;
        }
            
        if(_stuck2ndPool.transform.childCount > 0)
        {
            return _stuck2ndPool.GetComponentInChildren<Animator>().GetCurrentAnimatorStateInfo(0).length;
        }

        return 0;
    }
}
