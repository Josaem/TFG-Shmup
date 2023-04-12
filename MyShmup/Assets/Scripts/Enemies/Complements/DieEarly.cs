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
        Invoke(nameof(Kill), _deathDelay + GetComponent<Enemy>()._delayUntilActive);
    }

    private void Kill()
    {
        if(GetComponent<Enemy>().HasDeathWaypoint())
        {
            GetComponent<Enemy>().Kill();
        }
        else
        {
            GetComponent<Enemy>().DieByWaypoint();
        }
    }
}
