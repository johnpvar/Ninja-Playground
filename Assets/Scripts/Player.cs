using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PersonBehavior myPerson;

    // Start is called before the first frame update
    void Start()
    {
        myPerson = GetComponent<PersonBehavior>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        float deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * myPerson.MoveSpeed;
        float deltaY = Input.GetAxis("Vertical") * Time.deltaTime * myPerson.MoveSpeed;

        transform.position += new Vector3(deltaX, deltaY, 0);
    }
}
