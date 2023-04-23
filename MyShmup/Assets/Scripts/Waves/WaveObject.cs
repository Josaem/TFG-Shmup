using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour
{
    private Enemy[] _enemies;
    public float _duration;
    [HideInInspector]
    public BulletPool _bulletPool;
    private float _timer = 0;

    private void Awake()
    {
        _bulletPool = GetComponentInChildren<BulletPool>();
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;
        if(GetComponentsInChildren<Enemy>().Length == 0 && GetComponentsInChildren<Generator>().Length == 0 &&
            _bulletPool.transform.childCount == 0)
        {
            Destroy(gameObject);
        }
    }

    public int SetBasePriorityEnemies()
    {
        int _priorityEnemyCount = 0;

        _enemies = GetComponentsInChildren<Enemy>(false);
        foreach (Enemy enemy in _enemies)
        {
            if (enemy._prioritary)
            {
                _priorityEnemyCount++;
            }
        }

        return _priorityEnemyCount;
    }

    public void SetPriorityEnemies()
    {
        int _priorityEnemyCount = 0;

        _enemies = GetComponentsInChildren<Enemy>(false);
        foreach (Enemy enemy in _enemies)
        {
            if (enemy._prioritary && !enemy._isDead)
            {
                _priorityEnemyCount++;
            }
        }

        if (FindObjectOfType<GameManager>() != null)
        {
            FindObjectOfType<GameManager>()._priorityEnemiesLeft = _priorityEnemyCount;
        }
    }

    public void DespawnWave()
    {
        _enemies = GetComponentsInChildren<Enemy>();

        //If alive kills them
        foreach (Enemy enemy in _enemies)
        {
            enemy.Kill();
        }

        //If alive kills them
        foreach(Generator generator in GetComponentsInChildren<Generator>())
        {
            generator.EndSpawner();
        }
    }

    /*
    TODO SpecialMovement    
    */
}
