
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

    [Header("Sound Settings")]
    [SerializeField] private int maxSoundSources = 10;

    private Dictionary<string, AudioData> musicDictionary;
    private Dictionary<string, AudioData> soundDictionary;

    private List<AudioSource> soundSources = new List<AudioSource>();
    private List<AudioSource> pausedSources = new List<AudioSource>();

    private string currentMusic;
    private bool isMusicPaused;
    private bool isSoundPaused;

    protected override void Awake()
    {
        base.Awake();

        InitializeDictionary();
        InitializeSoundSources();
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

    private void AddToDictionary(
        List<AudioData> list,
        Dictionary<string, AudioData> dictionary)
    {
        foreach (AudioData audio in list)
        {
            if (audio == null || audio.clip == null ||
                string.IsNullOrEmpty(audio.audioName))
                continue;

            if (!dictionary.ContainsKey(audio.audioName))
                dictionary.Add(audio.audioName, audio);
        }
    }

    private void InitializeSoundSources()
    {
        soundSources.Clear();

        if (soundSource == null)
            return;

        soundSources.Add(soundSource);

        for (int i = 1; i < maxSoundSources; i++)
        {
            GameObject obj = new GameObject("SoundSource_" + i);
            obj.transform.SetParent(transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;

            soundSources.Add(source);
        }
    }

    public void PlayMusic(string audioName)
    {
        if (musicDictionary == null ||
            !musicDictionary.TryGetValue(audioName, out AudioData audio))
        {
            Debug.LogWarning("Music not found: " + audioName);
            return;
        }

        currentMusic = audioName;

        if (musicSource == null || !DataMusic.MusicEnabled)
            return;

        if (musicSource.clip == audio.clip)
        {
            if (isMusicPaused)
                return;

            if (!musicSource.isPlaying)
                musicSource.Play();

            return;
        }

        musicSource.Stop();
        musicSource.clip = audio.clip;
        musicSource.volume = audio.volume;
        musicSource.loop = audio.loop;

        isMusicPaused = false;

        musicSource.time = 0f;
        musicSource.Play();
    }

    public void PlaySound(string audioName)
    {
        if (!DataMusic.SoundEnabled)
            return;

        if (soundDictionary == null ||
            !soundDictionary.TryGetValue(audioName, out AudioData audio))
        {
            Debug.LogWarning("Sound not found: " + audioName);
            return;
        }

        AudioSource source = GetAvailableSoundSource();

        if (source == null)
            return;

        source.Stop();
        source.clip = audio.clip;
        source.volume = audio.volume;
        source.loop = audio.loop;
        source.Play();
    }

    private AudioSource GetAvailableSoundSource()
    {
        foreach (AudioSource source in soundSources)
        {
            if (source == null)
                continue;

            if (pausedSources.Contains(source))
                continue;

            if (!source.isPlaying)
                return source;
        }

        return null;
    }

    public void PauseAllSound()
    {
        isSoundPaused = true;
        pausedSources.Clear();

        foreach (AudioSource source in soundSources)
        {
            if (source != null && source.isPlaying)
            {
                source.Pause();
                pausedSources.Add(source);
            }
        }
    }

    public void ResumeAllSound()
    {
        isSoundPaused = false;

        foreach (AudioSource source in pausedSources)
        {
            if (source != null)
                source.UnPause();
        }

        pausedSources.Clear();
    }

    public void StopAllSound()
    {
        foreach (AudioSource source in soundSources)
        {
            if (source != null)
                source.Stop();
        }

        pausedSources.Clear();
        isSoundPaused = false;
    }

    public void StopSound()
    {
        StopAllSound();
    }

    public void SetMusicEnabled(bool enabled)
    {
        DataMusic.MusicEnabled = enabled;

        if (musicSource == null)
            return;

        if (!enabled)
        {
            StopMusic();
            return;
        }

        if (musicSource.clip != null)
        {
            if (isMusicPaused)
            {
                isMusicPaused = false;
                musicSource.UnPause();
            }
            else if (!musicSource.isPlaying)
            {
                musicSource.Play();
            }
        }
        else if (!string.IsNullOrEmpty(currentMusic))
        {
            PlayMusic(currentMusic);
        }
    }

    public void StopMusic()
    {
        if (musicSource == null)
            return;

        isMusicPaused = true;
        musicSource.Pause();
    }

    public void SetSoundEnabled(bool enabled)
    {
        DataMusic.SoundEnabled = enabled;

        if (!enabled)
            StopAllSound();
    }

    public void SetMusicVolume(string audioName, float volume)
    {
        if (musicDictionary == null ||
            !musicDictionary.TryGetValue(audioName, out AudioData audio))
            return;

        audio.volume = Mathf.Clamp01(volume);

        if (musicSource != null && musicSource.clip == audio.clip)
            musicSource.volume = audio.volume;
    }

    public void SetSoundVolume(string audioName, float volume)
    {
        if (soundDictionary == null ||
            !soundDictionary.TryGetValue(audioName, out AudioData audio))
            return;

        audio.volume = Mathf.Clamp01(volume);

        foreach (AudioSource source in soundSources)
        {
            if (source != null && source.clip == audio.clip)
                source.volume = audio.volume;
        }
    }

    public void PlayHaptic()
    {
        if (DataMusic.HapticEnabled)
            Handheld.Vibrate();
    }
}
