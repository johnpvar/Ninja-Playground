using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunter : MonoBehaviour
{
    [SerializeField] PersonBehavior currentPray; //visible only for debug
    [SerializeField] float eatingRange = 1.15f;
    PersonBehavior myPerson;

    public PersonBehavior CurrentPray { get => currentPray; set => currentPray = value; }


    private void Start()
    {
        myPerson = GetComponent<PersonBehavior>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    //Hunter specific
    public void Hunt()
    {
        currentPray = PickPray();
        if (currentPray == null) { return; }
        ChaseTarget();
        TryToEat();
    }


    private void ChaseTarget()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 prayPos = currentPray.transform.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, prayPos, myPerson.MoveSpeed * Time.deltaTime);
        transform.position = newPos;
    }


    public PersonBehavior PickPray()
    {
        var personList = FindObjectsOfType<PersonBehavior>();
        float closestDistance = Mathf.Infinity;
        PersonBehavior pray = null;
        foreach (var person in personList)
        {
            if (person.Role == 1) continue;

            float distance = Vector2.Distance(transform.position, person.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                pray = person;
            }
        }

        return pray;
    }


    public void TryToEat()
    {
        if (Vector2.Distance(transform.position, currentPray.transform.position) < eatingRange)
        {
            Eat();
        }
    }


    private void Eat()
    {
        int numPray = myPerson.GameManager.CountPray();
        if (numPray == myPerson.GameManager.RoundInitialPray)
        {
            Kill();
            if (!myPerson.PlayerControlled)
            {
                StartCoroutine(myPerson.Confuse(0.5f));
            }
        }
        else if (numPray > 1)
        {
            Convert();
            if (!myPerson.PlayerControlled)
            {
                StartCoroutine(myPerson.Confuse(0.5f));
            }
        }
        else
        {
            Kill();
            myPerson.GameManager.SetGameState();
            myPerson.GameManager.NextRound();
        }

        myPerson.GameManager.SetGameState();
    }


    private void Convert()
    {
        AudioSource.PlayClipAtPoint(myPerson.SoundManager.convertSFX, Camera.main.transform.position);
        currentPray.SetAsHunter(2f);
    }


    private void Kill()
    {
        AudioSource.PlayClipAtPoint(myPerson.SoundManager.killSFX, Camera.main.transform.position);
        currentPray.gameObject.SetActive(false);
        Destroy(currentPray.gameObject);
        GameObject dyingAnim = Instantiate(myPerson.PrayDying, currentPray.transform.position, Quaternion.identity);
        Destroy(dyingAnim, 1f);
    }
}
