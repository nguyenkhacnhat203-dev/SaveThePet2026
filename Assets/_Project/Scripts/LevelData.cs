using UnityEngine;

public static class LevelData
{
    private const string CURRENT_LEVEL_KEY = "CurrentLevel";

    public static void SaveLevel(int level)
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, level);
        PlayerPrefs.Save();
    }

    public static int LoadLevel()
    {
        return PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);
    }
}