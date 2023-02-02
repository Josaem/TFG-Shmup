using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LifeUI : MonoBehaviour
{
    [SerializeField] private GameObject _lifeSprite;


    // Start is called before the first frame update
    private void Start()
    {   
        SetUpLifebar();
    }

    public void SetUpLifebar()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < GameProperties._life; i++)
        {
            Instantiate(_lifeSprite,
                new Vector3(transform.position.x + (_lifeSprite.GetComponent<RectTransform>().rect.width / 2)*i, transform.position.y, 0),
                Quaternion.identity,
                transform);
        }
    }
}
