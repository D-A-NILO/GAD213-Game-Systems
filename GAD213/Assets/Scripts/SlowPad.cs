using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowPad : MonoBehaviour
{
    public float slowAmount = 10f; // how much to reduce speed
    public float duration = 2f;

    private Coroutine activeEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        MovementSystem move = other.GetComponent<MovementSystem>();
        Sliding slide = other.GetComponent<Sliding>();

        if (move != null)
        {
            if (activeEffect != null)
                StopCoroutine(activeEffect);

            activeEffect = StartCoroutine(ApplySlow(move, slide));
        }
    }

    private IEnumerator ApplySlow(MovementSystem move, Sliding slide)
    {
        float baseSpeed = move.baseSpeed;

        // Apply slow (clamp so it doesn’t go below a minimum speed)
        move.moveSpeed = Mathf.Max(1f, baseSpeed - slowAmount);

        // If sliding, reduce slide force too
        if (slide != null && slide.sliding)
        {
            StartCoroutine(ReduceSlideForce(slide, slowAmount * 0.5f, duration));
        }

        yield return new WaitForSeconds(duration);

        // Reset to normal
        move.moveSpeed = baseSpeed;

        activeEffect = null;
    }

    private IEnumerator ReduceSlideForce(Sliding slide, float reduceAmount, float duration)
    {
        slide.slideForce = Mathf.Max(1f, slide.slideForce - reduceAmount);
        yield return new WaitForSeconds(duration);
        slide.slideForce += reduceAmount;
    }
}
