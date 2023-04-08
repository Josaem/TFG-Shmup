using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating(nameof(SetUpScore), 0, 0.2f);
    }
    public void SetUpScore()
    {
        _scoreText.text = GameProperties._score.ToString("00000000000000000");
    }
}
