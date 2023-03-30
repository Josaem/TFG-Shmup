using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class Generator : MonoBehaviour
{
    [SerializeField]
    private float _delayUntilSpawn = 0;
    [SerializeField]
    private WaveSelector _delayDeathUntilWave;

    private bool _enabled = false;
    private bool _isDying = false;
    protected GameManager _myGM;

    [System.Serializable]
    private class WaveSelector
    {
        public int _section;
        public int _wave;
    }

    // Start is called before the first frame update
    private void Start()
    {
        _myGM = GetComponentInParent<GameManager>();
        Invoke(nameof(EnableSpawn), _delayUntilSpawn);
    }

    protected virtual void EnableSpawn()
    {
        _enabled = true;
    }

    public void EndSpawner()
    {
        if(_delayDeathUntilWave._section == 0 && _delayDeathUntilWave._wave == 0)
        {
            _enabled = false;
        }

        _isDying = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if(_enabled)
        {
            ManageSpawn();
        }

        if(_isDying && _myGM != null && (_myGM._sectionIndex == _delayDeathUntilWave._section && _myGM._waveIndex == _delayDeathUntilWave._wave))
        {
            _enabled = false;
        }
        
        if(!enabled && _isDying)
        {
            if (GetComponentsInChildren<Enemy>().Length == 0)
            {
                Destroy(gameObject);
            }
            else
            {
                foreach(Enemy enemy in GetComponentsInChildren<Enemy>())
                {
                    enemy.Kill();
                }
            }
        }
    }

    protected virtual void ManageSpawn()
    {

    }
}
