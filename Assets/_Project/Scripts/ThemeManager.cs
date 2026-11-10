
using System.Collections.Generic;
using UnityEngine;

public enum ThemeCategory
{
    Dog,
    Bee,
    Beehive
}

public class ThemeManager : Singleton<ThemeManager>
{
    private const string SAVE_KEY = "THEME_DATA";

    private ThemeData data;
    private DogTheme dogTheme;

    protected override void Awake()
    {
        base.Awake();

        LoadData();
        FindDogTheme();
    }

    private void FindDogTheme()
    {
        if (dogTheme != null)
            return;

        dogTheme = FindObjectOfType<DogTheme>(true);
    }

    public DogTheme GetDogTheme()
    {
        FindDogTheme();
        return dogTheme;
    }

    public void Initialize(
        List<ThemeItem> dogThemes,
        List<ThemeItem> beeThemes,
        List<ThemeItem> beehiveThemes)
    {
        bool changed = false;

        changed |= InitializeGroup(data.DogThemes, dogThemes);
        changed |= InitializeGroup(data.BeeThemes, beeThemes);
        changed |= InitializeGroup(data.BeehiveThemes, beehiveThemes);

        changed |= ValidateSelectedTheme(ThemeCategory.Dog, dogThemes);
        changed |= ValidateSelectedTheme(ThemeCategory.Bee, beeThemes);
        changed |= ValidateSelectedTheme(ThemeCategory.Beehive, beehiveThemes);

        if (changed)
            SaveData();

        ApplyDogTheme();
    }

    private bool InitializeGroup(
        List<bool> unlockedList,
        List<ThemeItem> themes)
    {
        if (themes == null)
            themes = new List<ThemeItem>();

        bool changed = false;

        while (unlockedList.Count > themes.Count)
        {
            unlockedList.RemoveAt(unlockedList.Count - 1);
            changed = true;
        }

        for (int i = 0; i < themes.Count; i++)
        {
            ThemeItem item = themes[i];

            if (i >= unlockedList.Count)
            {
                bool isFree = item != null &&
                              item.PriceType == ThemePriceType.Free;

                unlockedList.Add(isFree);
                changed = true;
            }
            else if (item != null &&
                     item.PriceType == ThemePriceType.Free &&
                     !unlockedList[i])
            {
                unlockedList[i] = true;
                changed = true;
            }
        }

        return changed;
    }

    public bool IsUnlocked(ThemeCategory category, int index)
    {
        List<bool> list = GetList(category);

        if (list == null || index < 0 || index >= list.Count)
            return false;

        return list[index];
    }

    public void UnlockTheme(ThemeCategory category, int index)
    {
        List<bool> list = GetList(category);

        if (list == null || index < 0 || index >= list.Count)
            return;

        if (list[index])
            return;

        list[index] = true;

        SaveData();
    }

    public int GetSelectedTheme(ThemeCategory category)
    {
        switch (category)
        {
            case ThemeCategory.Dog:
                return data.SelectedDogTheme;

            case ThemeCategory.Bee:
                return data.SelectedBeeTheme;

            case ThemeCategory.Beehive:
                return data.SelectedBeehiveTheme;
        }

        return 0;
    }

    public void SetSelectedTheme(ThemeCategory category, int index)
    {
        if (!IsUnlocked(category, index))
            return;

        switch (category)
        {
            case ThemeCategory.Dog:
                data.SelectedDogTheme = index;
                break;

            case ThemeCategory.Bee:
                data.SelectedBeeTheme = index;
                break;

            case ThemeCategory.Beehive:
                data.SelectedBeehiveTheme = index;
                break;
        }

        SaveData();

        if (category == ThemeCategory.Dog)
            ApplyDogTheme();
    }

    private bool ValidateSelectedTheme(
        ThemeCategory category,
        List<ThemeItem> themes)
    {
        if (themes == null || themes.Count == 0)
            return false;

        int selectedIndex = GetSelectedTheme(category);

        if (selectedIndex >= 0 &&
            selectedIndex < themes.Count &&
            IsUnlocked(category, selectedIndex))
            return false;

        for (int i = 0; i < themes.Count; i++)
        {
            if (IsUnlocked(category, i))
            {
                SetSelectedIndexWithoutSave(category, i);
                return true;
            }
        }

        return false;
    }

    private void SetSelectedIndexWithoutSave(
        ThemeCategory category,
        int index)
    {
        switch (category)
        {
            case ThemeCategory.Dog:
                data.SelectedDogTheme = index;
                break;

            case ThemeCategory.Bee:
                data.SelectedBeeTheme = index;
                break;

            case ThemeCategory.Beehive:
                data.SelectedBeehiveTheme = index;
                break;
        }
    }

    public void ApplyDogTheme()
    {
        FindDogTheme();

        if (dogTheme == null)
            return;

        int selectedIndex = data.SelectedDogTheme;

        if (!IsUnlocked(ThemeCategory.Dog, selectedIndex))
        {
            dogTheme.HideAll();
            return;
        }

        dogTheme.SetTheme(selectedIndex);
    }

    private List<bool> GetList(ThemeCategory category)
    {
        switch (category)
        {
            case ThemeCategory.Dog:
                return data.DogThemes;

            case ThemeCategory.Bee:
                return data.BeeThemes;

            case ThemeCategory.Beehive:
                return data.BeehiveThemes;
        }

        return null;
    }

    private void LoadData()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string json = PlayerPrefs.GetString(SAVE_KEY);
            data = JsonUtility.FromJson<ThemeData>(json);
        }

        if (data == null)
            data = new ThemeData();

        if (data.DogThemes == null)
            data.DogThemes = new List<bool>();

        if (data.BeeThemes == null)
            data.BeeThemes = new List<bool>();

        if (data.BeehiveThemes == null)
            data.BeehiveThemes = new List<bool>();
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(data);

        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public void ResetThemeData()
    {
        PlayerPrefs.DeleteKey(SAVE_KEY);

        data = new ThemeData();

        if (dogTheme != null)
            dogTheme.HideAll();
    }
}
