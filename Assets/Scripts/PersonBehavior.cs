using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBehavior : MonoBehaviour
{
    [SerializeField] Sprite hunterSprite;
    [SerializeField] Sprite praySprite;
    [SerializeField] int role = 0; //0 = pray, 1 = hunter
    [SerializeField] int behavior = 1; //0 = nothing, 1 = do the role

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float maxMoveSpeed = 10f;
    [SerializeField] float huntingRange = 3f;
    [SerializeField] float eatingRange = 0.1f;
    [SerializeField] PersonBehavior currentPray;
    GameManager gameManager;
    SoundManager soundManager;
    bool playerControlled = false;

    public int Role { get => role;}
    public int Behavior { get => behavior;}
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float MaxMoveSpeed { get => maxMoveSpeed; }
    public float HuntingRange { get => huntingRange; }
    public GameManager GameManager { get => gameManager; }

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        soundManager = FindObjectOfType<SoundManager>();
    }

    public void SetAsHunter(float delay)
    {
        GetComponent<SpriteRenderer>().sprite = hunterSprite;
        role = 1;
        if (!playerControlled && delay != 0f)
        {
            StartCoroutine(Confuse(delay));
        }
    }

    public void SetAsPray(float delay)
    {
        GetComponent<SpriteRenderer>().sprite = praySprite;
        role = 0;
        if (!playerControlled && delay != 0f)
        {
            StartCoroutine(Confuse(delay));
        }
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
        if (role == 1)
        {
            this.transform.GetChild(1).gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(seconds);
        behavior = 1;
        this.transform.GetChild(1).gameObject.SetActive(false);
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
            currentPray = PickPray();
            if (currentPray == null) { return; }
            TryToEat();
        }
    }

    private void ActAsRole()
    {
        if (behavior == 0) { return; }

        if (role == 0)
        {
            GetComponent<Pray>().PrayRole();
        }
        else
        {
            Hunt();
        }
    }



    //Hunter specific
    private void Hunt()
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
        Vector2 newPos = Vector2.MoveTowards(currentPos, prayPos, moveSpeed * Time.deltaTime);
        transform.position = newPos;
    }

    private PersonBehavior PickPray()
    {
        var personList = FindObjectsOfType<PersonBehavior>();
        float closestDistance = Mathf.Infinity;
        PersonBehavior pray = null;
        foreach (var person in personList)
        {
            if (person.role == 1) continue;

            float distance = Vector2.Distance(transform.position, person.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                pray = person;
            }
        }

        return pray;
    }

    private void TryToEat()
    {
        if (Vector2.Distance(transform.position, currentPray.transform.position) < eatingRange)
        {
            Eat();
        }
    }

    private void Eat()
    {
        int numPray = gameManager.CountPray();
        if (numPray == gameManager.RoundInitialPray)
        {
            Kill();
            if (!playerControlled)
            {
                StartCoroutine(Confuse(0.5f));
            }
        }
        else if (numPray > 1)
        {
            Convert();
            if (!playerControlled)
            {
                StartCoroutine(Confuse(0.5f));
            }
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
        currentPray.SetAsHunter(2f);
    }

    private void Kill()
    {
        AudioSource.PlayClipAtPoint(soundManager.killSFX, Camera.main.transform.position);
        currentPray.gameObject.SetActive(false);
        Destroy(currentPray.gameObject);
    }


}
