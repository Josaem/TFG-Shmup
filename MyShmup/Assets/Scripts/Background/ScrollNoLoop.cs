using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollNoLoop : MonoBehaviour
{
    [SerializeField]
    private float _scrollSpeedX;
    [SerializeField]
    private float _scrollSpeedY;

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(-_scrollSpeedX * Time.deltaTime, _scrollSpeedY * Time.deltaTime, 0);
    }
}
