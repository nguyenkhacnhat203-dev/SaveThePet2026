
using UnityEngine;

public static class LevelData
{
    private const string CURRENT_LEVEL_KEY = "CurrentLevel";
    private const string LEVEL_STARS_KEY = "LevelStars_";

    public static void SaveLevel(int level)
    {
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, level);
        PlayerPrefs.Save();

        //Debug.Log($"[LevelData] Saved Current Level: {level}");
    }

    public static int LoadLevel()
    {
        int level = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);

        //Debug.Log($"[LevelData] Loaded Current Level: {level}");

        return level;
    }

    public static void SaveLevelStars(int level, int starCount)
    {
        if (level <= 0)
            return;

        starCount = Mathf.Clamp(starCount, 0, 3);

        string key = LEVEL_STARS_KEY + level;

        int oldStars = PlayerPrefs.GetInt(key, 0);

        int newStars = Mathf.Max(oldStars, starCount);

        PlayerPrefs.SetInt(key, newStars);
        PlayerPrefs.Save();

        //Debug.Log(
        //    $"[LevelData] Level {level} | " +
        //    $"Old Stars: {oldStars} | " +
        //    $"New Stars: {newStars} | " +
        //    $"Achieved Stars: {starCount}"
        //);
    }

    public static int LoadLevelStars(int level)
    {
        if (level <= 0)
            return 0;

        string key = LEVEL_STARS_KEY + level;

        int stars = PlayerPrefs.GetInt(key, 0);

        //Debug.Log($"[LevelData] Loaded Level {level}: {stars} Stars");

        return stars;
    }
}
