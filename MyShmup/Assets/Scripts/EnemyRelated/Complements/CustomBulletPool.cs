using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomBulletPool : MonoBehaviour
{
    public Transform _newBP;

    void Awake()
    {
        _newBP = new GameObject("bp").transform;
        _newBP.SetParent(FindObjectOfType<BulletPool>().transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(_newBP != null)
            _newBP.position = transform.position;

        if(transform.childCount == 0)
            Destroy(_newBP.gameObject);
    }
}
