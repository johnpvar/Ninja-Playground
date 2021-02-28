using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pray : MonoBehaviour
{
    PersonBehavior myPerson;

    private void Start()
    {
        myPerson = GetComponent<PersonBehavior>();
    }


    public void PrayRole()
    {
        int numPray = myPerson.GameManager.CountPray();
        if (numPray == myPerson.GameManager.RoundInitialPray)
        {
            AvoidHunter();
        }
        else if (numPray > 1)
        {
            SeekHunter();
        }
        else
        {
            AvoidHunter();
        }
    }


    public PersonBehavior FindClosestHunter()
    {
        var hunterList = FindObjectsOfType<PersonBehavior>();
        float closestDistance = Mathf.Infinity;
        PersonBehavior hunter = null;
        foreach (var potentialHunter in hunterList)
        {
            if (potentialHunter.Role == 0) continue;

            float distance = Vector2.Distance(transform.position, potentialHunter.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                hunter = potentialHunter;
            }
        }

        return hunter;
    }


    private void AvoidHunter()
    {
        PersonBehavior hunter = FindClosestHunter();

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 hunterPos = hunter.transform.position;

        if (Vector2.Distance(currentPos, hunterPos) > myPerson.HuntingRange) { return; }

        Vector2 newPos = Vector2.MoveTowards(currentPos, hunterPos, -myPerson.MoveSpeed * Time.deltaTime); //the minus does the away thing
        transform.position = newPos;
    }


    private void SeekHunter()
    {
        PersonBehavior hunter = FindClosestHunter();

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 hunterPos = hunter.transform.position;

        Vector2 newPos = Vector2.MoveTowards(currentPos, hunterPos, myPerson.MoveSpeed * Time.deltaTime);
        transform.position = newPos;
    }

}
