using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public void ClearBullets()
    {
        foreach (BulletPool bp in FindObjectsOfType<BulletPool>())
        {
            bp.ClearOwnBullets();
        }
    }

    public void ClearOwnBullets()
    {
        foreach (WaveAttackBehavior wave in GetComponentsInChildren<WaveAttackBehavior>())
        {
            wave.Die();
        }

        foreach (EnemyBulletBehavior bullet in GetComponentsInChildren<EnemyBulletBehavior>())
        {
            bullet.Die();
        }
    }
}
