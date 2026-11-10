
using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThemeItem
{
    public Sprite Theme;
    public string Name_Theme;
    public ThemePriceType PriceType;
    public int BuyCoin;
    public int BuyGem;
}

[System.Serializable]
public class ThemeGroup
{
    public Transform Parent;
    public List<ThemeItem> Themes = new List<ThemeItem>();
}

public class PopupTheme : PopupBase
{
    [SerializeField] private BaseTheme prefab;

    [Header("Theme Groups")]
    [SerializeField] private ThemeGroup dogTheme;
    [SerializeField] private ThemeGroup beeTheme;
    [SerializeField] private ThemeGroup beehiveTheme;

    private BaseTheme currentThemeDog;
    private BaseTheme currentThemeBee;
    private BaseTheme currentThemeBeehive;

    private readonly List<BaseTheme> spawnedThemesDog = new List<BaseTheme>();
    private readonly List<BaseTheme> spawnedThemesBee = new List<BaseTheme>();
    private readonly List<BaseTheme> spawnedThemesBeehive = new List<BaseTheme>();

    private bool isSpawned;

    protected override void Start()
    {
        base.Start();
        Spawn();
    }

    public void Spawn()
    {
        if (isSpawned)
            return;

        if (prefab == null)
        {
            return;
        }

        SpawnThemes(dogTheme, spawnedThemesDog, SelectThemeDog, "Dog");
        SpawnThemes(beeTheme, spawnedThemesBee, SelectThemeBee, "Bee");
        SpawnThemes(beehiveTheme, spawnedThemesBeehive, SelectThemeBeehive, "Beehive");

        isSpawned = true;
    }

    private void SpawnThemes(
        ThemeGroup group,
        List<BaseTheme> list,
        Action<BaseTheme> callback,
        string groupName)
    {
        if (group == null)
        {
            return;
        }

        if (group.Parent == null)
        {
            return;
        }

        if (group.Themes == null || group.Themes.Count == 0)
        {
            return;
        }

        for (int i = group.Themes.Count - 1; i >= 0; i--)
        {
            ThemeItem item = group.Themes[i];

            if (item == null)
            {
                continue;
            }

            BaseTheme baseTheme = Instantiate(prefab, group.Parent);

            baseTheme.transform.SetAsFirstSibling();

            baseTheme.Setup(item, callback);

            list.Add(baseTheme);
        }

    }

    private void SelectThemeDog(BaseTheme selectedTheme)
    {
        SelectTheme(selectedTheme, spawnedThemesDog, ref currentThemeDog);
    }

    private void SelectThemeBee(BaseTheme selectedTheme)
    {
        SelectTheme(selectedTheme, spawnedThemesBee, ref currentThemeBee);
    }

    private void SelectThemeBeehive(BaseTheme selectedTheme)
    {
        SelectTheme(selectedTheme, spawnedThemesBeehive, ref currentThemeBeehive);
    }

    private void SelectTheme(
        BaseTheme selectedTheme,
        List<BaseTheme> list,
        ref BaseTheme currentTheme)
    {
        if (currentTheme == selectedTheme)
            return;

        foreach (BaseTheme theme in list)
        {
            if (theme != null)
                theme.SetTick(false);
        }

        currentTheme = selectedTheme;
        currentTheme.SetTick(true);
    }
}
