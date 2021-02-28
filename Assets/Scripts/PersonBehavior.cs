using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonBehavior : MonoBehaviour
{
    [SerializeField] Sprite hunterSprite;
    [SerializeField] Sprite praySprite;
    [SerializeField] GameObject prayDying;
    [SerializeField] int role = 0; //0 = pray, 1 = hunter
    [SerializeField] int behavior = 1; //0 = nothing, 1 = do the role

    [SerializeField] float moveSpeed = 10f;
    [SerializeField] float maxMoveSpeed = 10f;
    [SerializeField] float huntingRange = 3f;
    
    GameManager gameManager;
    SoundManager soundManager;
    bool playerControlled = false;

    public int Role { get => role;}
    public int Behavior { get => behavior;}
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    public float MaxMoveSpeed { get => maxMoveSpeed; }
    public float HuntingRange { get => huntingRange; }
    public GameManager GameManager { get => gameManager; }
    public SoundManager SoundManager { get => soundManager; }
    public bool PlayerControlled { get => playerControlled; }
    public GameObject PrayDying { get => prayDying; }


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
        //GetComponent<Player>().enabled = true;
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
            GetComponent<Player>().PlayerRole();
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
            GetComponent<Hunter>().Hunt();
        }
    }

    
}
