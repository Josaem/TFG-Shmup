using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour
{
    public float _duration;
    [HideInInspector]
    public BulletPool _bulletPool;
    public float _timer = 0;

    private bool _isPriorityWave = false;
    private int _totalEnemies = 0;
    private int _totalPriorityEnemies = 0;
    private int _totalGenerators = 0;

    private void Awake()
    {
        _bulletPool = GetComponentInChildren<BulletPool>();
    }

    public bool IsPriorityWave()
    {
        return _isPriorityWave;
    }

    public int CheckPriorityEnemies()
    {
        return _totalPriorityEnemies;
    }

    // Update is called once per frame
    void Update()
    {
        _timer += Time.deltaTime;

        if(_totalEnemies == 0 && _totalGenerators == 0 &&
            _bulletPool.transform.childCount == 0 && _timer > 3)
        {
            Destroy(gameObject);
        }
    }

    public void EnemySpawned()
    {
        _totalEnemies++;
    }

    public void EnemyDied()
    {
        _totalEnemies--;
    }

    public void PriorityEnemySpawned()
    {
        _totalPriorityEnemies++;
        _isPriorityWave = true;
    }

    public void PriorityEnemyDied()
    {
        _totalPriorityEnemies--;
    }

    public void GeneratorSpawned()
    {
        _totalGenerators++;
    }

    public void GeneratorDied()
    {
        _totalGenerators--;
    }

    public void DespawnWave()
    {
        //If alive kills them
        foreach (Enemy enemy in GetComponentsInChildren<Enemy>())
        {
            if(enemy._dieByWave)
                enemy.Kill();
        }

        //If alive kills them
        foreach(Generator generator in GetComponentsInChildren<Generator>())
        {
            if (generator._dieThisWave)
                generator.DisableSpawner();
            generator.KillSpawner();
        }
    }
}
