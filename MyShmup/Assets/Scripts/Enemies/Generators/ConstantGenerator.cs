using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class ConstantGenerator : Generator
{
    [SerializeField]
    private ObjectToGenerate[] _objectsToGenerate;

    private float _spawnTime = 0;

    [System.Serializable]
    private class ObjectToGenerate
    {
        public GameObject _enemy;
        public float _genDelay;
        public Transform _spawnPos;
    }

    protected override void ManageSpawn()
    {
        foreach(ObjectToGenerate genObject in _objectsToGenerate)
        {
            if (Time.time > _spawnTime + genObject._genDelay)
            {
                Instantiate(genObject._enemy, transform);
                _spawnTime = Time.time;
            }
        }
    }
}
