
using UnityEngine;

public static class LevelData
{
    private const string CURRENT_LEVEL_KEY = "CurrentLevel";
    private const string LEVEL_STARS_KEY = "LevelStars_";

    public static void SaveLevel(int level)
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, level);
        PlayerPrefs.Save();
    }

    public static int LoadLevel()
    {
        return PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);
    }

    public static int SaveLevelStars(int level, int starCount)
    {
        if (level <= 0)
            return 0;

        starCount = Mathf.Clamp(starCount, 0, 3);

        string key = LEVEL_STARS_KEY + level;

        int oldStars = PlayerPrefs.GetInt(key, 0);

        int newStars = Mathf.Max(oldStars, starCount);

        int earnedStars = newStars - oldStars;

        if (earnedStars > 0)
        {
            PlayerPrefs.SetInt(key, newStars);
            PlayerPrefs.Save();
        }

        return earnedStars;
    }

    public static int LoadLevelStars(int level)
    {
        if (level <= 0)
            return 0;

        return PlayerPrefs.GetInt(LEVEL_STARS_KEY + level, 0);
    }
}
