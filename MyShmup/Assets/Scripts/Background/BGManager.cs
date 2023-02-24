using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGManager : MonoBehaviour
{
    [SerializeField]
    private float _timer;

    public bool _waiting = false;

    [System.Serializable]
    public class BGObject
    {
        public GameObject _backgroundSprite;
        public float _time;
        public int _action;
        public bool _complete;
    }

    [SerializeField]
    private BGObject[] _bgObjectList;

    // Start is called before the first frame update
    void Start()
    {
        foreach (BGObject bgObject in _bgObjectList)
        {
            //If timer is ahead of event
            if (bgObject._time < _timer)
            {
                bgObject._complete = true;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!_waiting) _timer += Time.deltaTime;

        foreach (BGObject bgObject in _bgObjectList)
        {
            //If timer is ahead of event
            if (!bgObject._complete && bgObject._time < _timer)
            { 
                bgObject._complete = true;

                switch (bgObject._action)
                {
                    case 0: bgObject._backgroundSprite.SetActive(true);
                        break;

                    case 1: bgObject._backgroundSprite.SetActive(false);
                        break;

                    case 2: bgObject._backgroundSprite.GetComponent<ScrollNoLoop>().Action();
                        break;

                    default: Destroy(bgObject._backgroundSprite);
                        break;
                }
            }
        }
    }
}
