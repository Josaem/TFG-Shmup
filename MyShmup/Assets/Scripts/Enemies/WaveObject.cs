using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour
{
    private Enemy[] _enemies;
    private int _priorityEnemyCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        _enemies = GetComponentsInChildren<Enemy>();
        foreach (Enemy enemy in _enemies)
        {
            if (enemy._prioritary)
            {
                _priorityEnemyCount++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponentsInChildren<Enemy>().Length == 0)
        {
            Destroy(gameObject);
        }

        //TODO movement
    }

    public int CountPriorityEnemies()
    {
        return _priorityEnemyCount;
    }

    public void DespawnWave()
    {
        //If alive kills them
        foreach(Enemy enemy in _enemies)
        {
            enemy.Kill();
        }

        //TODO kill generators
    }

    public void PriorityEnemyKilled()
    {
        _priorityEnemyCount--;
    }

    /*
    TODO SpecialMovement    
    */
}
