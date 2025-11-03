using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedPad : MonoBehaviour, IInteractable
{
    public float speedBoost = 10f;
    public float duration = 2f;

    public void OnPlayerEnter(GameObject player)
    {
        MovementSystem move = player.GetComponent<MovementSystem>();
        if (move != null)
        {
            move.StartCoroutine(SpeedBoost(move));
        }
    }

    private IEnumerator SpeedBoost(MovementSystem move)
    {
        float originalSpeed = move.moveSpeed;
        move.moveSpeed += speedBoost;
        yield return new WaitForSeconds(duration);
        move.moveSpeed = originalSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {
        OnPlayerEnter(other.gameObject);
    }
}
