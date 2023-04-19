using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnWave : MonoBehaviour
{
    [SerializeField]
    private WaveSelector _waveToDie;

    private GameManager _myGm;

    [System.Serializable]
    public class WaveSelector
    {
        public int sectionIndex = 0;
        public int waveIndex = 0;
    }

    private void Start()
    {
        _myGm = GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    private void Update()
    {
        if(_myGm != null && _waveToDie.sectionIndex == _myGm._sectionIndex && _waveToDie.waveIndex == _myGm._waveIndex)
        {
            Kill();
        }
    }

    private void Kill()
    {
        if (TryGetComponent<Enemy>(out Enemy enemy))
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
        else
        {
            Destroy(gameObject);
        }
    }
}
