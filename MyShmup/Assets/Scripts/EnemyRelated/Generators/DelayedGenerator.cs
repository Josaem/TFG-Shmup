using UnityEngine;

public class DelayedGenerator : Generator
{
    [SerializeField]
    private GameObject[] _objectsToGenerate;

    public float _waitUntilSpawn;

    private bool _allowSpawn = true;

    protected override void EnableSpawn()
    {
        base.EnableSpawn();
        foreach (GameObject genObject in _objectsToGenerate)
        {
            Instantiate(genObject, transform);
        }
    }

    protected override void ManageSpawn()
    {
        if(_enabled && _allowSpawn && GetComponentsInChildren<Enemy>().Length == 0)
        {
            Invoke(nameof(SpawnEnemies), _waitUntilSpawn);
            _allowSpawn = false;
        }
    }

    private void SpawnEnemies()
    {
        foreach (GameObject genObject in _objectsToGenerate)
        {
            Instantiate(genObject, transform);
        }
        _allowSpawn = true;
    }
}
