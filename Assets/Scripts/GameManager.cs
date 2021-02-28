using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] int initialVictims = 10;
    [SerializeField] GameObject person;
    [SerializeField] TextMeshProUGUI instructionsText;
    [SerializeField] TextMeshProUGUI roundText;
    [SerializeField] GameObject winScreen;
    [SerializeField] GameObject lossScreen;
    [SerializeField] SoundManager soundManager;

    [Header("Round info")]
    [SerializeField] int round = 1; //debug
    [SerializeField] int roundInitialVictims; //debug
    [SerializeField] float forceRangeX = 10f;
    [SerializeField] float forceRangeY = 10f;
    [SerializeField] float forceCounterHunter = 50f;

    PersonBehavior player;
    bool isGameOver = false;

    
    public int Round { get => round; set => round = value; }
    public int RoundInitialVictims { get => roundInitialVictims; }


    // Start is called before the first frame update
    void Start()
    {
        SpawnHunter();
        SpawnVictims();
        roundInitialVictims = CountVictims();
        SetPlayer();
    }

    private void SetPlayer()
    {
        var persons = FindObjectsOfType<PersonBehavior>();
        int playerIndex = UnityEngine.Random.Range(0, persons.Length);
        player = persons[playerIndex];
        player.SetAsPlayerControlled();
        SetGameState();
    }

    public void SetGameState()
    {
        if (player == null || player.gameObject == null || !player.gameObject.activeInHierarchy)
        {
            if (!isGameOver)
            {
                instructionsText.text = "Game lost";
                soundManager.SetClipByName("loseMusic");
                instructionsText.gameObject.SetActive(false);
                lossScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    string.Format("You were in the last {0} survivors\nPress 'P' to play again", FindObjectsOfType<PersonBehavior>().Length + 1);
                lossScreen.SetActive(true);
            }
            isGameOver = true;
            return;
        }
        else if (FindObjectsOfType<PersonBehavior>().Length == 1)
        {
            instructionsText.text = "Game won";
            soundManager.SetClipByName("winMusic");
            instructionsText.gameObject.SetActive(false);
            winScreen.SetActive(true);
            player.transform.position = new Vector2(0f, 0f);
            return;
        }

        if (player.Role == 1)
        {
            instructionsText.text = "Get them!";
            soundManager.SetClipByName("huntMusic");
        }
        else if (player.Role == 0)
        {
            if (CountVictims() == roundInitialVictims)
            {
                instructionsText.text = "Run away!";
                soundManager.SetClipByName("avoidMusic");
            }
            else if (CountVictims() < roundInitialVictims && CountVictims() > 1)
            {
                instructionsText.text = "Join the dark side!";
                soundManager.SetClipByName("joinDarkMusic");
            }
            else
            {
                instructionsText.text = "Doomed...";
                soundManager.SetClipByName("avoidMusic");
            }
        }
        
    }

    private void SpawnHunter()
    {
        Vector2 pos = new Vector2(0, 0);
        var currentPerson = Instantiate(person, pos, Quaternion.identity);
        currentPerson.GetComponent<PersonBehavior>().SetAsHunter(2f);
        currentPerson.gameObject.name = "Person 0";
    }

    private void SpawnVictims()
    {
        for (int i = 0; i < initialVictims; i++)
        {
            float posX = UnityEngine.Random.Range(-6f, 6f);
            float posY = UnityEngine.Random.Range(-5f, 5f);

            Vector2 pos = new Vector2(posX, posY);
            var currentPerson = Instantiate(person, pos, Quaternion.identity);
            currentPerson.GetComponent<PersonBehavior>().SetAsVictim(0.5f);
            currentPerson.gameObject.name = "Person " + (i +1).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectPlayAgain();
    }

    private void DetectPlayAgain()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("Game");
        }
        
    }

    public int CountVictims()
    {
        int count = 0;
        var personList = FindObjectsOfType<PersonBehavior>();
        foreach (var person in personList)
        {
            if (person.gameObject.activeInHierarchy && person.Role == 0)
            {
                count++;
            }
        }
        return count;
    }

    public void NextRound()
    {
        List<PersonBehavior> personList = new List<PersonBehavior>(FindObjectsOfType<PersonBehavior>());
        foreach (var person in personList)
        {
            if (!person.gameObject.activeInHierarchy)
            {
                personList.Remove(person);
            }
            person.SetAsVictim(0f);
        }

        if (personList.Count <= 3 && !isGameOver)
        {
            player.SetAsHunter(0f);
        }
        else
        {
            int hunterIndex = UnityEngine.Random.Range(0, personList.Count);
            personList[hunterIndex].SetAsHunter(2.5f);
            Debug.LogFormat("Round {0} starting hunter is {1}", round, personList[hunterIndex].name);
        }
        
        foreach (var person in personList)
        {
            if (person.Role == 0)
            {
                AddRandomForce(person);
            }
            
        }

        round++;
        roundText.text = string.Format("Round {0}", round.ToString());
        Debug.LogFormat("Round {0} is starting with {1} survivors", round, personList.Count);
        roundInitialVictims = CountVictims();
    }

    private void AddRandomForce(PersonBehavior person)
    {
        float forceX = UnityEngine.Random.Range(-forceRangeX, forceRangeX);
        float forceY = UnityEngine.Random.Range(-forceRangeY, forceRangeY);
        Vector2 force = new Vector2(forceX, forceY);
        Vector2 forceNew = Vector2.MoveTowards(person.transform.position, person.FindClosestHunter().transform.position, -forceCounterHunter);
        person.gameObject.GetComponent<Rigidbody2D>().AddForce(forceNew, ForceMode2D.Impulse);
    }
}
