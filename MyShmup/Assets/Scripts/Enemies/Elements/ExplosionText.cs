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

    public void Spawn()
    {
        GetComponent<TMP_Text>().text = _score + " x " + _enemiesKilled;
        Invoke(nameof(Die), _timeToDie);
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
