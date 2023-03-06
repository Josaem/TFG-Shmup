using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int CountPriorityEnemies()
    {
        return 0;
    }

    public void DespawnWave()
    {
        Destroy(gameObject);
    }

    /*
    DespawnEnemies
    SpecialMovement
    
    On start it searches for children with component enemy
    
    */
}
