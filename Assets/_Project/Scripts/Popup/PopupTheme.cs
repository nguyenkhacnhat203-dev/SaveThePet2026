
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

[System.Serializable]
public class ThemeTab
{
    public Button Button;
    public Image Image;
    public Sprite On;
    public Sprite Off;
    public GameObject ScrollView;
}

public class PopupTheme : PopupBase
{
    [SerializeField] private BaseTheme prefab;

    [Header("Theme Groups")]
    [SerializeField] private ThemeGroup dogTheme;
    [SerializeField] private ThemeGroup beeTheme;
    [SerializeField] private ThemeGroup beehiveTheme;

    [Header("Theme Tabs")]
    [SerializeField] private ThemeTab dogTab;
    [SerializeField] private ThemeTab beeTab;
    [SerializeField] private ThemeTab beehiveTab;

    [Header("Purchase Question")]
    [SerializeField] private GameObject question;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private Action confirmPurchase;

    private BaseTheme currentThemeDog;
    private BaseTheme currentThemeBee;
    private BaseTheme currentThemeBeehive;

    private readonly List<BaseTheme> spawnedThemesDog = new List<BaseTheme>();
    private readonly List<BaseTheme> spawnedThemesBee = new List<BaseTheme>();
    private readonly List<BaseTheme> spawnedThemesBeehive = new List<BaseTheme>();

    private bool isSpawned;

    [SerializeField] private TMP_Text Coin;
    [SerializeField] private TMP_Text Gem;

    protected override void Start()
    {
        base.Start();

        SetupQuestion();
        Spawn();
        SetupTabs();
        SelectTab(0);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        GameAction.OnResourceChanged += UpdateResourceUI;

        if (ResourceManager.Instance != null)
        {
            UpdateResourceUI(
                ResourceManager.Instance.Coin,
                ResourceManager.Instance.Gem
            );
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        GameAction.OnResourceChanged -= UpdateResourceUI;

        HideQuestion();
    }

    public void ShowPopupCoin()
    {
        PopupManager.Instance.ShowPopupAddCoin();
    }
    public void ShowPopupGem()
    {
        PopupManager.Instance.ShowPopupAddGem();
    }

    private void SetupQuestion()
    {
        if (question != null)
            question.SetActive(false);

        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();

        yesButton.onClick.AddListener(ConfirmPurchase);
        noButton.onClick.AddListener(HideQuestion);
    }

    public void ShowQuestion(string themeName, Action callback)
    {
        if (question == null)
            return;

        confirmPurchase = callback;

        questionText.text = $"Are you ready to buy a {themeName}?";

        question.SetActive(true);
    }

    private void ConfirmPurchase()
    {
        Action callback = confirmPurchase;

        confirmPurchase = null;

        question.SetActive(false);

        callback?.Invoke();
    }

    private void HideQuestion()
    {
        confirmPurchase = null;

        if (question != null)
            question.SetActive(false);
    }

    private void UpdateResourceUI(int coin, int gem)
    {
        if (Coin != null)
            Coin.text = coin.ToString();

        if (Gem != null)
            Gem.text = gem.ToString();
    }

    public override void DestroyPopup()
    {
        AudioManager.Instance.PlaySound("TapButton");

        Destroy(gameObject);
    }

    public void Spawn()
    {
        if (isSpawned || prefab == null)
            return;

        ThemeManager manager = ThemeManager.Instance;

        if (manager == null)
        {
            Debug.LogError("ThemeManager chưa được khởi tạo!");
            return;
        }

        manager.Initialize(
            dogTheme.Themes,
            beeTheme.Themes,
            beehiveTheme.Themes
        );

        SpawnThemes(dogTheme, spawnedThemesDog, ThemeCategory.Dog, SelectThemeDog);
        SpawnThemes(beeTheme, spawnedThemesBee, ThemeCategory.Bee, SelectThemeBee);
        SpawnThemes(beehiveTheme, spawnedThemesBeehive, ThemeCategory.Beehive, SelectThemeBeehive);

        RestoreSelectedTheme(spawnedThemesDog, ThemeCategory.Dog, SelectThemeDog);
        RestoreSelectedTheme(spawnedThemesBee, ThemeCategory.Bee, SelectThemeBee);
        RestoreSelectedTheme(spawnedThemesBeehive, ThemeCategory.Beehive, SelectThemeBeehive);

        isSpawned = true;
    }

    private void SpawnThemes(
        ThemeGroup group,
        List<BaseTheme> list,
        ThemeCategory category,
        Action<BaseTheme> callback)
    {
        if (group == null || group.Parent == null ||
            group.Themes == null || group.Themes.Count == 0)
            return;

        ThemeManager manager = ThemeManager.Instance;

        for (int i = group.Themes.Count - 1; i >= 0; i--)
        {
            ThemeItem item = group.Themes[i];

            if (item == null)
                continue;

            int themeIndex = i;

            bool unlocked = manager.IsUnlocked(category, themeIndex);

            BaseTheme baseTheme = Instantiate(prefab, group.Parent);

            baseTheme.transform.SetAsFirstSibling();

            baseTheme.Setup(
                item,
                unlocked,
                callback,
                () => manager.UnlockTheme(category, themeIndex),
                ShowQuestion
            );

            list.Insert(0, baseTheme);
        }
    }

    private void RestoreSelectedTheme(
        List<BaseTheme> list,
        ThemeCategory category,
        Action<BaseTheme> callback)
    {
        if (list == null || list.Count == 0)
            return;

        int selectedIndex = ThemeManager.Instance.GetSelectedTheme(category);

        if (selectedIndex < 0 || selectedIndex >= list.Count)
            return;

        BaseTheme selected = list[selectedIndex];

        if (selected != null)
            callback?.Invoke(selected);
    }

    private void SetupTabs()
    {
        dogTab.Button.onClick.RemoveAllListeners();
        beeTab.Button.onClick.RemoveAllListeners();
        beehiveTab.Button.onClick.RemoveAllListeners();

        dogTab.Button.onClick.AddListener(() => SelectTab(0));
        beeTab.Button.onClick.AddListener(() => SelectTab(1));
        beehiveTab.Button.onClick.AddListener(() => SelectTab(2));
    }

    private void SelectTab(int index)
    {
        UpdateTab(dogTab, index == 0);
        UpdateTab(beeTab, index == 1);
        UpdateTab(beehiveTab, index == 2);
    }

    private void UpdateTab(ThemeTab tab, bool active)
    {
        if (tab == null)
            return;

        if (tab.ScrollView != null)
            tab.ScrollView.SetActive(active);

        if (tab.Image != null)
            tab.Image.sprite = active ? tab.On : tab.Off;
    }

    private void SelectThemeDog(BaseTheme selectedTheme)
    {
        SelectTheme(selectedTheme, spawnedThemesDog, ref currentThemeDog, ThemeCategory.Dog);
    }

    private void SelectThemeBee(BaseTheme selectedTheme)
    {
        SelectTheme(selectedTheme, spawnedThemesBee, ref currentThemeBee, ThemeCategory.Bee);
    }

    private void SelectThemeBeehive(BaseTheme selectedTheme)
    {
        SelectTheme(selectedTheme, spawnedThemesBeehive, ref currentThemeBeehive, ThemeCategory.Beehive);
    }

    private void SelectTheme(
        BaseTheme selectedTheme,
        List<BaseTheme> list,
        ref BaseTheme currentTheme,
        ThemeCategory category)
    {
        if (selectedTheme == null || currentTheme == selectedTheme)
            return;

        int index = list.IndexOf(selectedTheme);

        if (index < 0)
            return;

        foreach (BaseTheme theme in list)
        {
            if (theme != null)
                theme.SetTick(false);
        }

        currentTheme = selectedTheme;
        currentTheme.SetTick(true);

        ThemeManager.Instance.SetSelectedTheme(category, index);
    }
}
