using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour, IInteractable
{
    public float speedBoost = 10f;
    public float duration = 2f;

    private Coroutine activeBoost;

    //when the player touches object applies speed boost
    //it also checks if the player already has the speed boost active and makes sure it doesnt trigger again so that the player can't stack speed boosts
    public void onPlayerEnter(GameObject player)
    {
        MovementSystem move = player.GetComponent<MovementSystem>();
        Sliding slide = player.GetComponent<Sliding>();

        if (move != null)
        {
            //stop old boost if one is already running
            if (activeBoost != null)
                StopCoroutine(activeBoost);

            activeBoost = StartCoroutine(ApplySpeedBoost(move, slide));
        }
    }

    //when player triggers, call the onPlayerEnter function
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlayerEnter(other.gameObject);
        }
    }

    //applies temporary speed boost, then resets it
    private IEnumerator ApplySpeedBoost(MovementSystem move, Sliding slide)
    {
        float baseSpeed = move.baseSpeed;
        move.moveSpeed = baseSpeed + speedBoost;

        //if player is sliding, boost slide force as well
        if (slide != null && slide.sliding)
        {
            slide.BoostSlide(speedBoost * 0.5f, duration);
        }

        yield return new WaitForSeconds(duration);

        //reset to base speed
        move.moveSpeed = baseSpeed;

        activeBoost = null;
    }
}
