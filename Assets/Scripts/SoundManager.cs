using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] AudioClip huntMusic;
    [SerializeField] AudioClip avoidMusic;
    [SerializeField] AudioClip joinDarkMusic;
    [SerializeField] AudioClip loseMusic;
    [SerializeField] AudioClip winMusic;
    [SerializeField] AudioClip tutorialMusic;

    [Header("SoundFX")]
    [SerializeField] public AudioClip killSFX;
    [SerializeField] public AudioClip convertSFX;
    [SerializeField] [Range(0f, 1f)] float musicVolume;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<AudioSource>().volume = musicVolume;
    }

    public void SetClipByName(string clipName)
    {
        if (clipName == "huntMusic")
        {
            SetClip(huntMusic);
        }
        else if (clipName == "avoidMusic")
        {
            SetClip(avoidMusic);
        }
        else if (clipName == "joinDarkMusic")
        {
            SetClip(joinDarkMusic);
        }
        else if (clipName == "loseMusic")
        {
            SetClip(loseMusic);
        }
        else if (clipName == "winMusic")
        {
            SetClip(winMusic);
        }
        else if (clipName == "tutorialMusic")
        {
            SetClip(tutorialMusic);
        }
        else
        {
            SetClip(null);
        }
        
    }

    private void SetClip(AudioClip clip)
    {
        if (GetComponent<AudioSource>().clip != clip)
        {
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();
        }
    }
}
