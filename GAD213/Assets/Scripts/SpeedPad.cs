using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour, IInteractable
{
    public float speedBoost = 10f;
    public float duration = 2f;

    private Coroutine activeBoost;

    public void onPlayerEnter()
    { 
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        MovementSystem move = other.GetComponent<MovementSystem>();
        Sliding slide = other.GetComponent<Sliding>();

        if (move != null)
        {
            // Stop old boost if one is already running
            if (activeBoost != null)
                StopCoroutine(activeBoost);

            activeBoost = StartCoroutine(ApplySpeedBoost(move, slide));
        }
    }

    private IEnumerator ApplySpeedBoost(MovementSystem move, Sliding slide)
    {
        float baseSpeed = move.baseSpeed; // <-- store permanent base speed instead
        move.moveSpeed = baseSpeed + speedBoost;

        // If player is sliding, boost slide force as well
        if (slide != null && slide.sliding)
        {
            slide.BoostSlide(speedBoost * 0.5f, duration);
        }

        yield return new WaitForSeconds(duration);

        // Reset to base speed, not last "original" snapshot
        move.moveSpeed = baseSpeed;

        activeBoost = null;
    }
}
