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
    public bool _dieThisWave = true;

    protected bool _enabled = false;
    private bool _isDying = false;
    protected GameManager _myGM;

    // Start is called before the first frame update
    private void Start()
    {
        _myGM = FindObjectOfType<GameManager>();
        Invoke(nameof(EnableSpawn), _delayUntilSpawn);
    }

    protected virtual void EnableSpawn()
    {
        _enabled = true;
    }
    public void DisableSpawner()
    {
        _enabled = false;
    }

    public void KillSpawner()
    {
        _isDying = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if(_enabled)
        {
            ManageSpawn();
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
