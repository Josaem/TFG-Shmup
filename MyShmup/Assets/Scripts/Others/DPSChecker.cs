using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DPSChecker : MonoBehaviour
{
    public float totalDamage;
    private float elapsedTime;
    private TMP_Text _floatingText;

    private float windowDuration = 1f; // Time window duration in seconds
    private float windowStartTime; // Start time of the current time window
    private float damageInWindow; // Accumulated damage within the time window

    private void Start()
    {
        _floatingText = GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    public void TakeDamage(float damage)
    {
        // Check if the current time is beyond the time window
        if (Time.time > windowStartTime + windowDuration)
        {
            CalculateDPS(); // Calculate and display the DPS for the previous time window
            ResetWindow(); // Reset the window to start tracking a new time window
        }

        // Accumulate the damage within the time window
        damageInWindow += damage;
    }

    // Resets the time window and accumulated damage
    private void ResetWindow()
    {
        windowStartTime = Time.time;
        damageInWindow = 0f;
    }

    // Calculates and displays the DPS for the previous time window
    private void CalculateDPS()
    {
        float dps = damageInWindow / windowDuration;
        _floatingText.text = dps.ToString();
    }
}
