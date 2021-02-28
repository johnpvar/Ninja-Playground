using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sand : MonoBehaviour
{
    [SerializeField] [Range(0f, 1f)] float slowdownRatio = 0.15f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<PersonBehavior>().MoveSpeed *= (1f - slowdownRatio);
        Debug.LogFormat("{0} has entered the sand ", collision.gameObject.name);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collision.gameObject.GetComponent<PersonBehavior>().MoveSpeed = collision.gameObject.GetComponent<PersonBehavior>().MaxMoveSpeed;
        Debug.LogFormat("{0} exited sand ", collision.gameObject.name);
    }
}
