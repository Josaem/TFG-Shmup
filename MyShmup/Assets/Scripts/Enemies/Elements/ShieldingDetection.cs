using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldingDetection : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Enemy>().IsShielded();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<Enemy>().IsNotShielded();
    }
}
