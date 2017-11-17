using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicPlayer : MonoBehaviour {

    int musicCurrent = 0;
    AudioSource audioSource;
    public AudioClip[] clipNames;
    public Text musicName;
    public Slider musicLength;
    public Slider musicVolume;
    private bool stop = true;
    bool audioMuted = false;
    bool audioPaused = false;
    bool audioRepeat = false;



	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
        musicVolume.value = 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		if(!stop && !audioPaused)
        {
            if (!audioMuted)
            {
                audioSource.volume = musicVolume.value;
            }
            musicLength.value += Time.deltaTime;
            if(musicLength.value >= audioSource.clip.length)
            {
                if (!audioRepeat)
                {
                    musicCurrent++;
                    if (musicCurrent >= clipNames.Length)
                    {
                        musicCurrent = 0;
                    }
                }
                StartAudio();
            }
        }
	}
    public void StartAudio(int changeMusic = 0)
    {
        musicCurrent += changeMusic;

        if (audioPaused && changeMusic == 0) //if paused and not changing to a different track simply resume playing
        {
            audioSource.UnPause();
            audioPaused = false;
            return;
        }
        else
        {
            audioPaused = false; //or just unpause and carry on changing to the new track
        }

        if(musicCurrent >= clipNames.Length)
        {
            musicCurrent = 0;
        }
        else if(musicCurrent < 0)
        {
            musicCurrent = clipNames.Length - 1;
        }

        if(audioSource.isPlaying && changeMusic == 0)
        {
            return;
        }
        
        if(stop)
        {
            stop = false;
        }

        audioSource.clip = clipNames[musicCurrent];
        musicName.text = audioSource.clip.name;
        musicLength.maxValue = audioSource.clip.length;
        musicLength.value = 0;
        audioSource.Play();
    }
    public void StopAudio()
    {
        audioSource.Stop();
        stop = true;
    }

    public void MuteAudio()
    {
        if (audioMuted == false)
        {
            audioMuted = true;
            audioSource.volume = 0.0f;
        }
        else
        {
            audioMuted = false;
            audioSource.volume = musicVolume.value;
        }

    }

    public void PauseAudio()
    {
        if(!audioPaused)
        {
            audioSource.Pause();
            audioPaused = true;
        }
        else
        {
            audioSource.UnPause();
            audioPaused = false;
        }
    }

    public void RepeatAudio()
    {
        if(!audioRepeat)
        {
            audioRepeat = true;
        }
        else
        {
            audioRepeat = false;
        }

    }


}

