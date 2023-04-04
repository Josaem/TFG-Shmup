using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour
{
    private Enemy[] _enemies;

    // Update is called once per frame
    void Update()
    {
        if(GetComponentsInChildren<Enemy>().Length == 0 && GetComponentsInChildren<Generator>().Length == 0)
        {
            Destroy(gameObject);
        }
    }

    public void SetPriorityEnemies()
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

        if (FindObjectOfType<GameManager>() != null)
        {
            FindObjectOfType<GameManager>()._priorityEnemiesLeft = _priorityEnemyCount;
        }
    }

    public void SetPriorityEnemiesDead()
    {
        int _priorityEnemyCount = -1;

        _enemies = GetComponentsInChildren<Enemy>(false);
        foreach (Enemy enemy in _enemies)
        {
            if (enemy._prioritary)
            {
                _priorityEnemyCount++;
            }
        }

        Debug.Log(_priorityEnemyCount);

        if(FindObjectOfType<GameManager>() != null)
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
