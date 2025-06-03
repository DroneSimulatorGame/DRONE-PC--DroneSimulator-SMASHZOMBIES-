using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.Audio;

public class MainScript : MonoBehaviour
{




    public static MainScript instance;

    public List<AudioClip> clips = new List<AudioClip>();
    AudioSource musicSource;
    public AudioMixer SFX_Mixer;
    public AudioMixerGroup StartAndLoopMixerGroup;


    bool fadeout = false;

    public AudioSource audioFX_start, audioFX_loop, audioFX_end;
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        musicSource = this.GetComponent<AudioSource>();
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeout)
        {
            float currentVolume;
            StartAndLoopMixerGroup.audioMixer.GetFloat("StartLoopGroup_Volume", out currentVolume);
            StartAndLoopMixerGroup.audioMixer.SetFloat("StartLoopGroup_Volume", currentVolume - 500f * Time.deltaTime);
        }
    }

    public void PressBtn(int N)
    {
        fadeout = false;
        StartAndLoopMixerGroup.audioMixer.SetFloat("StartLoopGroup_Volume", 0f);
        float randomPitch = Random.Range(0.97f, 1.03f);
        SFX_Mixer.SetFloat("SFX_Pitch", randomPitch);
        audioFX_start.clip = clips[3 * N - 3];
        audioFX_loop.clip = clips[3 * N - 2];
        double startTime = AudioSettings.dspTime;
        audioFX_start.PlayScheduled(startTime);
        audioFX_loop.PlayScheduled(startTime + audioFX_start.clip.length / randomPitch);
    }

    public void ReleaseBtn(int N)
    {
        fadeout = true;
        audioFX_end.clip = clips[3 * N - 1];
        audioFX_end.Play();
    }
}
