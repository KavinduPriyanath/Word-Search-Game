using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioClip buttonTouch;
    public AudioClip notSelectSound;
    public AudioClip selectSound;
    public AudioClip letterPickSound;
    public AudioClip victorySound;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {

            Destroy(gameObject);
        }
    }

    public void PlayClip(AudioClip clip)
    {
        effectsSource.clip = clip;
        effectsSource.Play();
    }

    public void PlayTapSound()
    {
        effectsSource.clip = buttonTouch;
        effectsSource.Play();
    }
}
