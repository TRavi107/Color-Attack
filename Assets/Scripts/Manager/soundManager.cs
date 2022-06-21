using RamailoGames;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum SoundType
{
    none,
    mainMenuSound,
    backgroundSound,
    slashSound,
    uiSound,
    pauseSound,
    explosion,
    hit,
    shot,
}


public class soundManager : MonoBehaviour
{
    public static soundManager instance;
    public List<AudioClip> mainMenuSound;
    public List<AudioClip> backGroundSound;
    public List<AudioClip> uiSounds;
    public List<AudioClip> shootSound;
    public List<AudioClip> explosionSound;
    public List<AudioClip> hitSound;

    public AudioClip pauseResumeSound;
    public float backGroundAudioVolume;
    public float soundeffectVolume;

    private AudioSource UISoundSource;
    private AudioSource backGroundAudioSource;
    private AudioSource effectSoundSource;
    private AudioSource shootSoundSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        MusicVolumeChanged(backGroundAudioVolume);
        SoundVolumeChanged(soundeffectVolume);
        
        SceneManager.LoadScene(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (backGroundAudioSource != null)
        {
            if(backGroundAudioSource.isPlaying == false)
            {
                switch (SceneManager.GetActiveScene().buildIndex)
                {
                    
                    case 1:
                        PlaySound(SoundType.mainMenuSound);
                        break;
                    case 2:
                        PlaySound(SoundType.backgroundSound);
                        break;
                    
                }
                MusicVolumeChanged(backGroundAudioVolume);
                SoundVolumeChanged(soundeffectVolume);
            }
        }
    }

    public void PlaySound(SoundType soundType)
    {
        AudioClip clip;
        int soundIndex;
        switch (soundType)
        {
            case SoundType.mainMenuSound:
                soundIndex = Random.Range(0, mainMenuSound.Count);
                clip = mainMenuSound[soundIndex];
                if (backGroundAudioSource == null)
                {
                    backGroundAudioSource = gameObject.AddComponent<AudioSource>();
                }
                backGroundAudioSource.clip = clip;
                backGroundAudioSource.loop = false;
                backGroundAudioSource.Play();
                break;
            case SoundType.backgroundSound:
                soundIndex = Random.Range(0, backGroundSound.Count);
                clip = backGroundSound[soundIndex];
                if (backGroundAudioSource == null)
                {
                    backGroundAudioSource = gameObject.AddComponent<AudioSource>();
                }
                backGroundAudioSource.clip = clip;
                backGroundAudioSource.loop = false;
                backGroundAudioSource.Play();
                break;

            case SoundType.uiSound:
                soundIndex = Random.Range(0, uiSounds.Count);
                clip = uiSounds[soundIndex];
                if (UISoundSource == null)
                {
                    UISoundSource = gameObject.AddComponent<AudioSource>();
                }
                UISoundSource.clip = clip;
                UISoundSource.loop = false;
                UISoundSource.Play();
                break;
            case SoundType.pauseSound:
                clip = pauseResumeSound;
                if (UISoundSource == null)
                {
                    UISoundSource = gameObject.AddComponent<AudioSource>();
                }
                UISoundSource.clip = clip;
                UISoundSource.loop = false;
                UISoundSource.Play();
                break;

            case SoundType.shot:
                
                soundIndex = Random.Range(0, shootSound.Count);
                clip = shootSound[soundIndex];
                if (shootSoundSource == null)
                {
                    shootSoundSource = gameObject.AddComponent<AudioSource>();
                }
                shootSoundSource.clip = clip;
                shootSoundSource.loop = false;
                shootSoundSource.Play();
                break;
            case SoundType.explosion:
                soundIndex = Random.Range(0, explosionSound.Count);
                clip = explosionSound[soundIndex];
                if (effectSoundSource == null)
                {
                    effectSoundSource = gameObject.AddComponent<AudioSource>();
                }
                effectSoundSource.clip = clip;
                effectSoundSource.loop = false;
                effectSoundSource.Play();
                break;
            case SoundType.hit:
                soundIndex = Random.Range(0, hitSound.Count);
                clip = hitSound[soundIndex];
                if (effectSoundSource == null)
                {
                    effectSoundSource = gameObject.AddComponent<AudioSource>();
                }
                effectSoundSource.clip = clip;
                effectSoundSource.loop = false;
                effectSoundSource.Play();
                break;
            default:
                break;
        }
    }

    public void MusicVolumeChanged(float volume)
    {
        if(backGroundAudioSource != null)
        {
            backGroundAudioSource.volume = volume;
        }
    }
    public void SoundVolumeChanged(float volume)
    {
        if (effectSoundSource != null)
        {
            effectSoundSource.volume = volume;
        }
    }

    public void SaveMusicVoulme(float volume)
    {
        backGroundAudioVolume = volume;
    }
    public void SaveSoundVoulme(float volume)
    {
        soundeffectVolume = volume;
    }
}
