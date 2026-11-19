
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

    [Header("Pagination")]
    [SerializeField] private Button Btn_Left;
    [SerializeField] private Button Btn_Right;
    [SerializeField] private Image Img_Left;
    [SerializeField] private Image Img_Right;
    [SerializeField] private int themesPerPage = 10;
    [SerializeField] private byte disabledAlpha = 150;

    [Header("Purchase Question")]
    [SerializeField] private GameObject question;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    [Header("Resources")]
    [SerializeField] private TMP_Text Coin;
    [SerializeField] private TMP_Text Gem;

    private Action confirmPurchase;

    private BaseTheme currentThemeDog;
    private BaseTheme currentThemeBee;
    private BaseTheme currentThemeBeehive;

    private readonly List<BaseTheme> spawnedThemesDog = new List<BaseTheme>();
    private readonly List<BaseTheme> spawnedThemesBee = new List<BaseTheme>();
    private readonly List<BaseTheme> spawnedThemesBeehive = new List<BaseTheme>();

    private int currentTab = -1;

    private int dogPage;
    private int beePage;
    private int beehivePage;

    private int totalPages;

    private bool isSpawned;

    protected override void Start()
    {
        base.Start();

        themesPerPage = Mathf.Max(1, themesPerPage);

        SetupQuestion();
        SetupTabs();
        SetupPagination();

        Spawn();
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

    private void SetupQuestion()
    {
        if (question != null)
            question.SetActive(false);

        if (yesButton != null)
        {
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(ConfirmPurchase);
        }

        if (noButton != null)
        {
            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(HideQuestion);
        }
    }

    private void SetupTabs()
    {
        if (dogTab.Button != null)
        {
            dogTab.Button.onClick.RemoveAllListeners();
            dogTab.Button.onClick.AddListener(() => SelectTab(0));
        }

        if (beeTab.Button != null)
        {
            beeTab.Button.onClick.RemoveAllListeners();
            beeTab.Button.onClick.AddListener(() => SelectTab(1));
        }

        if (beehiveTab.Button != null)
        {
            beehiveTab.Button.onClick.RemoveAllListeners();
            beehiveTab.Button.onClick.AddListener(() => SelectTab(2));
        }
    }

    private void SetupPagination()
    {
        if (Btn_Left != null)
        {
            Btn_Left.onClick.RemoveAllListeners();
            Btn_Left.onClick.AddListener(PreviousPage);
        }

        if (Btn_Right != null)
        {
            Btn_Right.onClick.RemoveAllListeners();
            Btn_Right.onClick.AddListener(NextPage);
        }
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

        isSpawned = true;

        SelectTab(0);
    }

    private void SelectTab(int index)
    {
        if (!isSpawned || index < 0 || index > 2)
            return;

        if (currentTab == index)
            return;
        //AudioManager.Instance.PlaySound("TapButton");

        currentTab = index;

        ClearAllSpawnedThemes();

        UpdateTab(dogTab, index == 0);
        UpdateTab(beeTab, index == 1);
        UpdateTab(beehiveTab, index == 2);

        SpawnCurrentTab();
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

    private void SpawnCurrentTab()
    {
        switch (currentTab)
        {
            case 0:
                SpawnThemes(
                    dogTheme,
                    spawnedThemesDog,
                    ThemeCategory.Dog,
                    dogPage,
                    SelectThemeDog
                );
                break;

            case 1:
                SpawnThemes(
                    beeTheme,
                    spawnedThemesBee,
                    ThemeCategory.Bee,
                    beePage,
                    SelectThemeBee
                );
                break;

            case 2:
                SpawnThemes(
                    beehiveTheme,
                    spawnedThemesBeehive,
                    ThemeCategory.Beehive,
                    beehivePage,
                    SelectThemeBeehive
                );
                break;
        }

        UpdateArrowUI();
    }


    private void SpawnThemes(
        ThemeGroup group,
        List<BaseTheme> list,
        ThemeCategory category,
        int page,
        Action<BaseTheme, int> callback)
    {
        if (group == null || group.Parent == null ||
            group.Themes == null || group.Themes.Count == 0)
        {
            totalPages = 0;
            return;
        }

        ThemeManager manager = ThemeManager.Instance;

        int startIndex = page * themesPerPage;

        int endIndex = Mathf.Min(
            startIndex + themesPerPage,
            group.Themes.Count
        );

        totalPages = Mathf.CeilToInt(
            (float)group.Themes.Count / themesPerPage
        );

        for (int i = endIndex - 1; i >= startIndex; i--)
        {
            ThemeItem item = group.Themes[i];

            if (item == null)
                continue;

            int themeIndex = i;

            bool unlocked = manager.IsUnlocked(category, themeIndex);

            BaseTheme baseTheme = Instantiate(
                prefab,
                group.Parent
            );

            baseTheme.transform.SetAsFirstSibling();

            baseTheme.Setup(
                item,
                unlocked,
                selected => callback?.Invoke(selected, themeIndex),
                () => manager.UnlockTheme(category, themeIndex),
                ShowQuestion,
                ShowPopupCoin,
                ShowPopupGem
            );

            list.Insert(0, baseTheme);
        }

        RestoreSelectedTheme(
            list,
            category,
            callback,
            startIndex,
            endIndex
        );
    }


    private void RestoreSelectedTheme(
        List<BaseTheme> list,
        ThemeCategory category,
        Action<BaseTheme, int> callback,
        int startIndex,
        int endIndex)
    {
        int selectedIndex = ThemeManager.Instance.GetSelectedTheme(category);

        if (selectedIndex < startIndex || selectedIndex >= endIndex)
            return;

        int localIndex = selectedIndex - startIndex;

        if (localIndex < 0 || localIndex >= list.Count)
            return;

        BaseTheme selected = list[localIndex];

        if (selected != null)
            callback?.Invoke(selected, selectedIndex);
    }

    private void ClearAllSpawnedThemes()
    {
        ClearList(spawnedThemesDog);
        ClearList(spawnedThemesBee);
        ClearList(spawnedThemesBeehive);

        currentThemeDog = null;
        currentThemeBee = null;
        currentThemeBeehive = null;
    }

    private void ClearList(List<BaseTheme> list)
    {
        foreach (BaseTheme theme in list)
        {
            if (theme != null)
                Destroy(theme.gameObject);
        }

        list.Clear();
    }

    private void PreviousPage()
    {
        if (currentTab < 0 || GetCurrentPage() <= 0)
            return;

        SetCurrentPage(GetCurrentPage() - 1);
        AudioManager.Instance.PlaySound("TapButton");
        ClearCurrentList();
        SpawnCurrentTab();
    }

    private void NextPage()
    {
        if (currentTab < 0 || GetCurrentPage() >= totalPages - 1)
            return;
        AudioManager.Instance.PlaySound("TapButton");
        SetCurrentPage(GetCurrentPage() + 1);

        ClearCurrentList();
        SpawnCurrentTab();
    }

    private int GetCurrentPage()
    {
        switch (currentTab)
        {
            case 0: return dogPage;
            case 1: return beePage;
            case 2: return beehivePage;
            default: return 0;
        }
    }

    private void SetCurrentPage(int page)
    {
        switch (currentTab)
        {
            case 0:
                dogPage = page;
                break;

            case 1:
                beePage = page;
                break;

            case 2:
                beehivePage = page;
                break;
        }
    }

    private void ClearCurrentList()
    {
        switch (currentTab)
        {
            case 0:
                ClearList(spawnedThemesDog);
                currentThemeDog = null;
                break;

            case 1:
                ClearList(spawnedThemesBee);
                currentThemeBee = null;
                break;

            case 2:
                ClearList(spawnedThemesBeehive);
                currentThemeBeehive = null;
                break;
        }
    }

    private void UpdateArrowUI()
    {
        bool canGoLeft = GetCurrentPage() > 0;
        bool canGoRight = GetCurrentPage() < totalPages - 1;

        UpdateArrowAlpha(Img_Left, canGoLeft);
        UpdateArrowAlpha(Img_Right, canGoRight);

        if (Btn_Left != null)
            Btn_Left.interactable = canGoLeft;

        if (Btn_Right != null)
            Btn_Right.interactable = canGoRight;
    }

    private void UpdateArrowAlpha(Image image, bool active)
    {
        if (image == null)
            return;

        Color color = image.color;

        color.a = active ? 1f : disabledAlpha / 255f;

        image.color = color;
    }

    private void SelectThemeDog(BaseTheme selectedTheme, int index)
    {
        SelectTheme(
            selectedTheme,
            spawnedThemesDog,
            ref currentThemeDog,
            ThemeCategory.Dog,
            index
        );
    }

    private void SelectThemeBee(BaseTheme selectedTheme, int index)
    {
        SelectTheme(
            selectedTheme,
            spawnedThemesBee,
            ref currentThemeBee,
            ThemeCategory.Bee,
            index
        );
    }

    private void SelectThemeBeehive(BaseTheme selectedTheme, int index)
    {
        SelectTheme(
            selectedTheme,
            spawnedThemesBeehive,
            ref currentThemeBeehive,
            ThemeCategory.Beehive,
            index
        );
    }

    private void SelectTheme(
        BaseTheme selectedTheme,
        List<BaseTheme> list,
        ref BaseTheme currentTheme,
        ThemeCategory category,
        int index)
    {
        if (selectedTheme == null || currentTheme == selectedTheme)
            return;

        if (index < 0)
            return;
        AudioManager.Instance.PlaySound("TapButton");

        foreach (BaseTheme theme in list)
        {
            if (theme != null)
                theme.SetTick(false);
        }

        currentTheme = selectedTheme;
        currentTheme.SetTick(true);

        ThemeManager.Instance.SetSelectedTheme(category, index);
    }

    public void ShowQuestion(string themeName, Action callback)
    {
        if (question == null)
            return;

        confirmPurchase = callback;

        if (questionText != null)
            questionText.text = $"Are you ready to buy a {themeName}?";

        question.SetActive(true);
    }

    private void ConfirmPurchase()
    {
        //AudioManager.Instance.PlaySound("TapButton");

        Action callback = confirmPurchase;

        confirmPurchase = null;

        if (question != null)
            question.SetActive(false);

        callback?.Invoke();
    }

    private void HideQuestion()
    {
        AudioManager.Instance.PlaySound("TapButton");

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

    public void ShowPopupCoin()
    {
        if (Coin != null)
            PopupManager.Instance.ShowPopupAddCoin(Coin.gameObject);
    }

    public void ShowPopupGem()
    {
        if (Gem != null)
            PopupManager.Instance.ShowPopupAddGem(Gem.gameObject);
    }

    public void Refresh()
    {
        ClearAllSpawnedThemes();

        dogPage = Mathf.Clamp(
            dogPage, 0,
            Mathf.Max(0, GetTotalPages(dogTheme) - 1)
        );

        beePage = Mathf.Clamp(
            beePage, 0,
            Mathf.Max(0, GetTotalPages(beeTheme) - 1)
        );

        beehivePage = Mathf.Clamp(
            beehivePage, 0,
            Mathf.Max(0, GetTotalPages(beehiveTheme) - 1)
        );

        if (currentTab >= 0)
            SpawnCurrentTab();
    }

    private int GetTotalPages(ThemeGroup group)
    {
        if (group == null || group.Themes == null)
            return 0;

        return Mathf.CeilToInt(
            (float)group.Themes.Count / Mathf.Max(1, themesPerPage)
        );
    }

    public override void DestroyPopup()
    {
        AudioManager.Instance.PlaySound("TapButton");

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (dogTab.Button != null)
            dogTab.Button.onClick.RemoveAllListeners();

        if (beeTab.Button != null)
            beeTab.Button.onClick.RemoveAllListeners();

        if (beehiveTab.Button != null)
            beehiveTab.Button.onClick.RemoveAllListeners();

        if (Btn_Left != null)
            Btn_Left.onClick.RemoveListener(PreviousPage);

        if (Btn_Right != null)
            Btn_Right.onClick.RemoveListener(NextPage);
    }
}
