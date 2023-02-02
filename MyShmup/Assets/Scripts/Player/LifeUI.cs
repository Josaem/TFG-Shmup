using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LifeUI : MonoBehaviour
{
    [SerializeField] private GameObject _lifeSprite;
    [SerializeField] private GameObject[] _lifeArray;


    // Start is called before the first frame update
    private void Start()
    {
        //TODO change to the value of the static class
        _lifeArray = new GameObject[10];
        SetUpLifebar();
    }

    public void SetUpLifebar()
    {
        for (int i = 0; i < _lifeArray.Length; i++)
        {
            _lifeArray[i] = Instantiate(_lifeSprite,
                new Vector3(transform.position.x + (_lifeSprite.GetComponent<RectTransform>().rect.width / 2)*i, transform.position.y, 0),
                Quaternion.identity,
                transform) as GameObject;
        }
    }

    // Update is called once per frame
    public void LivesUpdate()
    {
        
    }
}
