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
        Invoke(nameof(Kill), _deathDelay);
    }

    private void Kill()
    {
        GetComponent<Enemy>().Kill();
    }
}
