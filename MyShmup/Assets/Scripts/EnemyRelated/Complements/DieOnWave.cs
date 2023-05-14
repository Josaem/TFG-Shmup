using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieOnWave : MonoBehaviour
{
    [SerializeField]
    private RelativeWave _relative;

    [SerializeField]
    private WaveSelector _waveToDie;

    private GameManager _myGm;

    [System.Serializable]
    public class RelativeWave
    {
        public bool relative;
        public int wavesToWait = 0;
    }

    [System.Serializable]
    public class WaveSelector
    {
        public int sectionIndex = 0;
        public int waveIndex = 0;
    }

    private void Start()
    {
        _myGm = FindObjectOfType<GameManager>();

        if (_relative.relative && _myGm != null)
        {
            _waveToDie.sectionIndex = _myGm._sectionIndex;
            _waveToDie.waveIndex = _myGm._waveIndex + _relative.wavesToWait + 1;
        }
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
        if (TryGetComponent(out Enemy enemy))
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
        else if (TryGetComponent(out Generator generator))
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
