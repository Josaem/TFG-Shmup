
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyHitAfterAnimation : MonoBehaviour
{
    private Animator animator;
    public float delay = 0.0f;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        StartCoroutine(DestroyObjectAfterAnimation());
    }

    IEnumerator DestroyObjectAfterAnimation()
    {
        // Wait until the animation is finished
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length + delay);

        // Destroy the object
        Destroy(gameObject);
    }
}

