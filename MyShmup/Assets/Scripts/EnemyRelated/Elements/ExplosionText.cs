using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ExplosionText : MonoBehaviour
{
    [SerializeField]
    private float _timeToDie;
    [HideInInspector]
    public int _score;
    [HideInInspector]
    public int _enemiesKilled;

    public void Spawn(bool outOfBounds)
    {
        TMP_Text myText = GetComponent<TMP_Text>();

        if (outOfBounds)
        {
            myText.fontSize = myText.fontSize / 2;
        }
        
        myText.text = _enemiesKilled + " x\n" + _score;
        Invoke(nameof(Die), _timeToDie);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
