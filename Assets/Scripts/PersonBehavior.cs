using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBehavior : MonoBehaviour
{
    [SerializeField] Sprite hunterSprite;
    [SerializeField] Sprite victimSprite;
    [SerializeField] int role = 0; //0 = victim, 1 = hunter
    [SerializeField] int behavior = 1; //0 = nothing, 1 = do the role

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float huntingRange = 3f;
    [SerializeField] float eatingRange = 0.1f;
    [SerializeField] PersonBehavior currentVictim;
    GameManager gameManager;
    SoundManager soundManager;
    bool playerControlled = false;

    public int Role { get => role;}
    public int Behavior { get => behavior;}

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void SetAsHunter()
    {
        GetComponent<SpriteRenderer>().sprite = hunterSprite;
        role = 1;
        StartCoroutine(Confuse(2f));
    }

    public void SetAsVictim()
    {
        GetComponent<SpriteRenderer>().sprite = victimSprite;
        role = 0;
        StartCoroutine(Confuse(0.5f));
    }

    public void SetAsPlayerControlled()
    {
        playerControlled = true;
        this.transform.GetChild(0).gameObject.SetActive(true);
        GetComponent<Player>().enabled = true;
    }

    public IEnumerator Confuse(float seconds)
    {
        behavior = 0;
        yield return new WaitForSeconds(seconds);
        behavior = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerControlled)
        {
            ActAsRole();
        }
        else
        {
            PlayerRole();
        }
    }

    private void PlayerRole()
    {
        if (role == 1)
        {
            currentVictim = PickTarget();
            if (currentVictim == null) { return; }
            TryToEat();
        }
    }

    private void ActAsRole()
    {
        if (behavior == 0) { return; }

        if (role == 0)
        {
            VictimRole();
        }
        else
        {
            Hunt();
        }
    }

    private void VictimRole()
    {
        int numVictims = gameManager.CountVictims();
        if (numVictims == gameManager.RoundInitialVictims)
        {
            AvoidHunter();
        }
        else if (numVictims > 1)
        {
            SeekHunter();
        }
        else
        {
            AvoidHunter();
        }
    }

    //Hunter specific
    private void Hunt()
    {
        currentVictim = PickTarget();
        if (currentVictim == null) { return; }
        ChaseTarget();
        TryToEat();
    }

    private void ChaseTarget()
    {
        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 victimPos = currentVictim.transform.position;
        Vector2 newPos = Vector2.MoveTowards(currentPos, victimPos, moveSpeed * Time.deltaTime);
        transform.position = newPos;
    }

    private PersonBehavior PickTarget()
    {
        var victimList = FindObjectsOfType<PersonBehavior>();
        float closestDistance = Mathf.Infinity;
        PersonBehavior victim = null;
        foreach (var potentialVictim in victimList)
        {
            if (potentialVictim.role == 1) continue;

            float distance = Vector2.Distance(transform.position, potentialVictim.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                victim = potentialVictim;
            }
        }

        return victim;
    }

    private void TryToEat()
    {
        if (Vector2.Distance(transform.position, currentVictim.transform.position) < eatingRange)
        {
            Eat();
        }
    }

    private void Eat()
    {
        int numVictims = gameManager.CountVictims();
        if (numVictims == gameManager.RoundInitialVictims)
        {
            Kill();
        }
        else if (numVictims > 1)
        {
            Convert();
        }
        else
        {
            Kill();
            gameManager.SetGameState();
            gameManager.NextRound();
        }

        gameManager.SetGameState();
    }

    private void Convert()
    {
        AudioSource.PlayClipAtPoint(soundManager.convertSFX, Camera.main.transform.position);
        currentVictim.SetAsHunter();
    }

    private void Kill()
    {
        AudioSource.PlayClipAtPoint(soundManager.killSFX, Camera.main.transform.position);
        currentVictim.gameObject.SetActive(false);
        Destroy(currentVictim.gameObject);
    }



    //Victim specific

    private PersonBehavior FindClosestHunter()
    {
        var hunterList = FindObjectsOfType<PersonBehavior>();
        float closestDistance = Mathf.Infinity;
        PersonBehavior hunter = null;
        foreach (var potentialHunter in hunterList)
        {
            if (potentialHunter.role == 0) continue;

            float distance = Vector2.Distance(transform.position, potentialHunter.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                hunter = potentialHunter;
            }
        }

        return hunter;
    }

    void AvoidHunter()
    {
        PersonBehavior hunter = FindClosestHunter();

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 hunterPos = hunter.transform.position;

        if (Vector2.Distance(currentPos, hunterPos) > huntingRange) { return; }

        Vector2 newPos = Vector2.MoveTowards(currentPos, hunterPos, -moveSpeed * Time.deltaTime); //the minus does the away thing
        transform.position = newPos;
    }

    void SeekHunter()
    {
        PersonBehavior hunter = FindClosestHunter();

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 hunterPos = hunter.transform.position;

        Vector2 newPos = Vector2.MoveTowards(currentPos, hunterPos, moveSpeed * Time.deltaTime);
        transform.position = newPos;
    }
}
