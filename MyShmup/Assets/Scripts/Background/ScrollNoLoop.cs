using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollNoLoop : MonoBehaviour
{
    [SerializeField]
    private float _scrollSpeedX;
    [SerializeField]
    private float _scrollSpeedY;

    [System.Serializable]
    public class ScrollObject
    {
        public float _scrollX;
        public float _scrollY;
    }

    [SerializeField]
    private ScrollObject[] _scrollSpeeds;

    private int index = 0;

    private void Start()
    {
        _scrollSpeedX = _scrollSpeeds[0]._scrollX;
        _scrollSpeedY = _scrollSpeeds[0]._scrollY;
    }

    // Update is called once per frame
    private void Update()
    {
        transform.position += new Vector3(-_scrollSpeedX * Time.deltaTime, -_scrollSpeedY * Time.deltaTime, 0);
    }

    public void Action()
    {
        index++;
        _scrollSpeedX = _scrollSpeeds[index]._scrollX;
        _scrollSpeedY = _scrollSpeeds[index]._scrollY;
    }
}
