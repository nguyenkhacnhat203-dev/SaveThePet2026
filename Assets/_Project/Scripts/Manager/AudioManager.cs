
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Serializable]
    public class AudioData
    {
        public string audioName;
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 1f;

        public bool loop;
    }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundSource;

    [Header("Music List")]
    [SerializeField] private List<AudioData> musicList = new List<AudioData>();

    [Header("Sound List")]
    [SerializeField] private List<AudioData> soundList = new List<AudioData>();

    private Dictionary<string, AudioData> musicDictionary;
    private Dictionary<string, AudioData> soundDictionary;

    private string currentMusic;

    protected override void Awake()
    {
        base.Awake();

        InitializeDictionary();
    }


    private void Start()
    {
        PlayMusic("BgMusic");
    }
    private void InitializeDictionary()
    {
        musicDictionary = new Dictionary<string, AudioData>();
        soundDictionary = new Dictionary<string, AudioData>();

        AddToDictionary(musicList, musicDictionary);
        AddToDictionary(soundList, soundDictionary);
    }

    private void AddToDictionary(List<AudioData> list, Dictionary<string, AudioData> dictionary)
    {
        foreach (AudioData audio in list)
        {
            if (audio == null || audio.clip == null || string.IsNullOrEmpty(audio.audioName))
                continue;

            if (!dictionary.ContainsKey(audio.audioName))
                dictionary.Add(audio.audioName, audio);
        }
    }

    public void PlayMusic(string audioName)
    {
        if (!DataMusic.MusicEnabled || musicSource == null)
            return;

        if (!musicDictionary.TryGetValue(audioName, out AudioData audio))
        {
            Debug.LogWarning("Music not found: " + audioName);
            return;
        }

        currentMusic = audioName;

        if (musicSource.clip == audio.clip && musicSource.isPlaying)
            return;

        musicSource.clip = audio.clip;
        musicSource.volume = audio.volume;
        musicSource.loop = audio.loop;
        musicSource.Play();
    }

    public void PlaySound(string audioName)
    {
        if (!DataMusic.SoundEnabled || soundSource == null)
            return;

        if (!soundDictionary.TryGetValue(audioName, out AudioData audio))
        {
            Debug.LogWarning("Sound not found: " + audioName);
            return;
        }

        if (audio.loop)
        {
            soundSource.clip = audio.clip;
            soundSource.volume = audio.volume;
            soundSource.loop = true;
            soundSource.Play();
        }
        else
        {
            soundSource.PlayOneShot(audio.clip, audio.volume);
        }
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void StopSound()
    {
        if (soundSource != null)
            soundSource.Stop();
    }

    public void SetMusicEnabled(bool enabled)
    {
        DataMusic.MusicEnabled = enabled;

        if (!enabled)
        {
            StopMusic();
        }
        else if (!string.IsNullOrEmpty(currentMusic))
        {
            PlayMusic(currentMusic);
        }
    }

    public void SetSoundEnabled(bool enabled)
    {
        DataMusic.SoundEnabled = enabled;

        if (!enabled)
            StopSound();
    }

    public void SetMusicVolume(string audioName, float volume)
    {
        if (!musicDictionary.TryGetValue(audioName, out AudioData audio))
            return;

        audio.volume = Mathf.Clamp01(volume);

        if (musicSource.clip == audio.clip)
            musicSource.volume = audio.volume;
    }

    public void SetSoundVolume(string audioName, float volume)
    {
        if (!soundDictionary.TryGetValue(audioName, out AudioData audio))
            return;

        audio.volume = Mathf.Clamp01(volume);
    }

    public void PlayHaptic()
    {
        if (DataMusic.HapticEnabled)
            Handheld.Vibrate();
    }
}
