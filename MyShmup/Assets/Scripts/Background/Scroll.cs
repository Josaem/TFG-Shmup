using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scroll : MonoBehaviour
{
    [SerializeField]
    private bool _vertical;
    [SerializeField]
    private float _scrollLimit;
    [SerializeField]
    private float _scrollSpeed;

    // Update is called once per frame
    void Update()
    {
        if (_vertical)
        {
            transform.position += new Vector3(0, -_scrollSpeed * Time.deltaTime, 0);

            if(transform.position.y <= _scrollLimit)
                transform.position = new Vector3(0, 0, 0);
        }
        else
        {
            transform.position += new Vector3(-_scrollSpeed * Time.deltaTime, 0, 0);

            if (transform.position.x <= _scrollLimit)
                transform.position = new Vector3(0, 0, 0);
        }
    }
}
