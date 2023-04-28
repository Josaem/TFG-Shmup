using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieEarly : MonoBehaviour
{
    [SerializeField]
    private float _deathDelay;

    // Start is called before the first frame update
    void Start()
    {
        if (TryGetComponent<Enemy>(out Enemy enemy))
        {

            Invoke(nameof(Kill), _deathDelay + enemy._delayUntilActive);
        }
        else
        {
            Invoke(nameof(Kill), _deathDelay);
        }
    }

    private void Kill()
    {
        if(TryGetComponent<Enemy>(out Enemy enemy))
        {
            if (enemy.HasDeathWaypoint())
            {
                enemy.Kill();
            }            
            else
            {
                enemy.DieByWaypoint();
            }
        }
        else if (TryGetComponent<Generator>(out Generator generator))
        {
            generator.DisableSpawner();
            generator.KillSpawner();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
