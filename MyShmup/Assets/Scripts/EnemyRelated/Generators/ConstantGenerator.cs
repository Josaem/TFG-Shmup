using UnityEngine;

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
    }

    protected override void ManageSpawn()
    {
        if(_enabled)
        {
            foreach (ObjectToGenerate genObject in _objectsToGenerate)
            {
                if (Time.time > _spawnTime + genObject._genDelay)
                {
                    Instantiate(genObject._enemy, transform);
                    _spawnTime = Time.time;
                }
            }
        }
    }
}
