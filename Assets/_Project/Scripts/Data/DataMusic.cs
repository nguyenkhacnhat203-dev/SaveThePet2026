
using UnityEngine;

public static class DataMusic
{
    private const string MusicKey = "Settings_Music";
    private const string SoundKey = "Settings_Sound";
    private const string HapticKey = "Settings_Haptic";

    public static bool MusicEnabled
    {
        get => PlayerPrefs.GetInt(MusicKey, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(MusicKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool SoundEnabled
    {
        get => PlayerPrefs.GetInt(SoundKey, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(SoundKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool HapticEnabled
    {
        get => PlayerPrefs.GetInt(HapticKey, 1) == 1;
        set
        {
            PlayerPrefs.SetInt(HapticKey, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
