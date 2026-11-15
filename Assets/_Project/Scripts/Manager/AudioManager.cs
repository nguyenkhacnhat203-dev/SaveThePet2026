
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
    private bool isMusicPaused;

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
        if (!DataMusic.SoundEnabled || soundSource == null)
            return;

        if (soundDictionary == null ||
            !soundDictionary.TryGetValue(audioName, out AudioData audio))
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
        if (musicSource == null)
            return;

        isMusicPaused = true;
        musicSource.Pause();
    }

    public void StopSound()
    {
        if (soundSource != null)
            soundSource.Stop();
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

    public void SetSoundEnabled(bool enabled)
    {
        DataMusic.SoundEnabled = enabled;

        if (!enabled)
            StopSound();
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
    }

    public void PlayHaptic()
    {
        if (DataMusic.HapticEnabled)
            Handheld.Vibrate();
    }
}
