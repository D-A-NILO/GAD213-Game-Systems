using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowPad : MonoBehaviour, IInteractable
{
    public float slowAmount = 10f;
    public float duration = 2f;

    private Coroutine activeEffect;

    //when player touches object apply slowness, if player is already affected by slowness dont apply again to prevent the effect stacking
    public void onPlayerEnter(GameObject player)
    {
        MovementSystem move = player.GetComponent<MovementSystem>();
        Sliding slide = player.GetComponent<Sliding>();

        if (move != null)
        {
            //stop old boost if one is already running
            if (activeEffect != null)
                StopCoroutine(activeEffect);

            activeEffect = StartCoroutine(ApplySlow(move, slide));
        }
    }

    //when player triggers, call onPlayerEnter function
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerEnter(other.gameObject);
        }
    }

    //applies temporary slowness, then resets it
    private IEnumerator ApplySlow(MovementSystem move, Sliding slide)
    {
        float baseSpeed = move.baseSpeed;

        //Apply slow, clamp so it doesn’t go below basespeed
        move.moveSpeed = Mathf.Max(1f, baseSpeed - slowAmount);

        //if sliding, reduce slide force too
        if (slide != null && slide.sliding)
        {
            StartCoroutine(ReduceSlideForce(slide, slowAmount * 0.5f, duration));
        }

        yield return new WaitForSeconds(duration);

        //reset to normal
        move.moveSpeed = baseSpeed;

        activeEffect = null;
    }

    //reduce slideforce temporarily
    private IEnumerator ReduceSlideForce(Sliding slide, float reduceAmount, float duration)
    {
        slide.slideForce = Mathf.Max(1f, slide.slideForce - reduceAmount);
        yield return new WaitForSeconds(duration);
        slide.slideForce += reduceAmount;
    }
}
