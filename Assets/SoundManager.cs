using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sound
{
    Rain,
    Sun,
    Wind,
    MaxCount
}

public class SoundManager : MonoBehaviour
{
    private AudioSource[] _audioSources = new AudioSource[(int)Sound.MaxCount];

    /* Singleton */
    private static SoundManager _instance;

    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SoundManager();
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        else
        {
            Destroy(this);
        }

        InitSound();
    }

    public void InitSound()
    {
        GameObject root = GameObject.Find("@SoundManager");

        if (root == null)
        {
            root = new GameObject("@SoundManager");
            Object.DontDestroyOnLoad(root);

            for (int i = 0; i < (int)Sound.MaxCount; i++)
            {
                GameObject go = new GameObject(System.Enum.GetNames(typeof(Sound))[i]);
                _audioSources[i] = go.AddComponent<AudioSource>();
                go.transform.parent = root.transform;
            }

            _audioSources[(int)Sound.Sun].loop = true;
            _audioSources[(int)Sound.Sun].volume = 0.3f;
            _audioSources[(int)Sound.Wind].loop = true;
            _audioSources[(int)Sound.Rain].loop = true;
        }
    }

    public void Play(AudioClip audioClip, Sound soundtype = Sound.Sun, float pitch = 1.0f)
    {
        if (audioClip == null)
            return;

        AudioSource audioSource = _audioSources[(int)soundtype];
        audioSource.pitch = pitch;

        if(audioSource.clip != audioClip)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    public void PlayWithSpatialBlend(AudioSource audioSource, AudioClip audioClip, float pitch = 1.0f)
    {
        audioSource.pitch = pitch;
        audioSource.spatialBlend = 1;
        audioSource.rolloffMode = AudioRolloffMode.Linear;
        audioSource.minDistance = 1;
        audioSource.maxDistance = 20;
        audioSource.PlayOneShot(audioClip);
    }

    #region Sound Resources
    public AudioClip[] rain;
    public AudioClip[] sun;
    public AudioClip[] wind;

    public void RainBGM(int i)
    {
        this.Play(rain[i], Sound.Rain);
    }

    public void SunBGM(int i)
    {
        this.Play(sun[i], Sound.Sun);
    }

    public void WindBGM(int i)
    {
        this.Play(wind[i], Sound.Wind);
    }
    #endregion
}