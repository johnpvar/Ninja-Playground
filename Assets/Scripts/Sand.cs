using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sand : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] float slowdownRatio = 0.15f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<PersonBehavior>().MoveSpeed *= (1f - slowdownRatio);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<PersonBehavior>().MoveSpeed = collision.gameObject.GetComponent<PersonBehavior>().MaxMoveSpeed;
    }
}
