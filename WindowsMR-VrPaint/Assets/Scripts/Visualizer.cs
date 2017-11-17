using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent (typeof (AudioSource))]

public class Visualizer : MonoBehaviour {
    public static float[] _samples = new float[512];
    public static float[] _freqBand = new float[8];
    public GameObject MusicPlayer;
    AudioSource audioSource;


    // Use this for initialization
    void Start () {
        audioSource = MusicPlayer.GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        GetSpectrumAudioSource();
        MakeFrequencyBands();
		
	}

  

    private void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(_samples, 0, FFTWindow.Blackman);
    }

    private void MakeFrequencyBands()
    {
        /*
         * 22050 / 512 = 43hertz per sample
         * 
         * 20 -60 hertz
         * 250 - 500 hz
         * 500 - 2000hz
         * 2000 - 4000hz
         * 4000 - 6000hz
         * 6000 - 20000hz
         * 
         * 0 - 2 = 86hz
         * 1 - 4 = 172hz - 87-258
         * 2 - 8 = 344hz - 259-602
         * 3 - 16 = 688hz - 603-1290
         * 4 - 32 = 1276hz - 1291-2666
         * 5 - 64 = 2752hz - 2667-5418
         * 6 -128 = 5504hz - 5419-10922
         * 7 -256 = 11008hz - 10923-21930
         * 510
         */
        int count = 0;
         for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sampleCount = (int)Mathf.Pow(2, i) * 2;

            if(i == 7)
            {
                sampleCount += 2;
            }

            for (int j = 0; j < sampleCount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;
            _freqBand[i] = average * 10;
        }
    }
}
