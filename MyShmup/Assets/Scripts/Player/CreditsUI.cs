using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _creditsText;
    // Start is called before the first frame update
    private void Start()
    {
        SetUpCredits();
    }
    public void SetUpCredits()
    {
        _creditsText.text = "CREDITS: " + GameProperties._credits.ToString();
    }
}
