using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioClip readyConfirmation;
    public AudioClip three;
    public AudioClip two;
    public AudioClip one;
    public AudioClip lookCommand;
    public AudioClip front;
    public AudioClip left;
    public AudioClip right;
    public AudioClip top;
    public AudioClip bottom;
    public AudioClip thankYou;
    public AudioClip recordDone;

    Dictionary<string, AudioClip> audios;

    private AudioSource audioSrc;

    // Use this for initialization
    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();

        audios = new Dictionary<string, AudioClip>();
        audios.Add("readyConf", readyConfirmation);
        audios.Add("three", three);
        audios.Add("two", two);
        audios.Add("one", one);
        audios.Add("lookCommand", lookCommand);
        audios.Add("front", front);
        audios.Add("left", left);
        audios.Add("right", right);
        audios.Add("top", top);
        audios.Add("bottom", bottom);
        audios.Add("thankYou", thankYou);
        audios.Add("recordDone", recordDone);
    }

    public void PlayAudio(string audioName) {
        audioSrc.PlayOneShot(audios[audioName]);
    }
}
