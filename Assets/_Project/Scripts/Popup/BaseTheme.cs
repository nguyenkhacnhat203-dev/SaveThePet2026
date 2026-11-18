
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ThemePriceType
{
    Free,
    BuyCoin,
    BuyGem
}

public class BaseTheme : MonoBehaviour
{
    public Image Theme;
    public Image Cover;
    public Image Tick;

    public TextMeshProUGUI Name_Theme;
    public TextMeshProUGUI Cost_BuyCoin;
    public TextMeshProUGUI Cost_BuyGem;

    public Button Btn_BuyCoin;
    public Button Btn_BuyGem;

    public Image Img_Theme;

    private ThemePriceType priceType;
    private Action<BaseTheme> onSelect;
    private Action unlockCallback;
    private Action<string, Action> showQuestion;

    private Button themeButton;

    private bool isUnlocked;

    private int buyCoin;
    private int buyGem;

    private string themeName;
    private Action showPopupCoin;
    private Action showPopupGem;
    public void Setup(
        ThemeItem data,
        bool unlocked,
        Action<BaseTheme> callback,
        Action onUnlock,
        Action<string, Action> questionCallback,
        Action popupCoinCallback,
        Action popupGemCallback)
    {
        if (data == null)
            return;

        if (Theme == null || Cover == null || Tick == null ||
            Name_Theme == null || Cost_BuyCoin == null ||
            Cost_BuyGem == null || Btn_BuyCoin == null ||
            Btn_BuyGem == null || Img_Theme == null)
            return;

        Theme.sprite = data.Theme;
        Name_Theme.text = data.Name_Theme;
        themeName = data.Name_Theme;

        priceType = data.PriceType;
        onSelect = callback;
        unlockCallback = onUnlock;
        showQuestion = questionCallback;

        showPopupCoin = popupCoinCallback;
        showPopupGem = popupGemCallback;

        buyCoin = data.BuyCoin;
        buyGem = data.BuyGem;

        isUnlocked = unlocked || priceType == ThemePriceType.Free;

        Cost_BuyCoin.text = buyCoin.ToString();
        Cost_BuyGem.text = buyGem.ToString();

        Btn_BuyCoin.onClick.RemoveAllListeners();
        Btn_BuyGem.onClick.RemoveAllListeners();

        Btn_BuyCoin.onClick.AddListener(BuyWithCoin);
        Btn_BuyGem.onClick.AddListener(BuyWithGem);

        UpdateUnlockUI();
        SetTick(false);
    }
    private void AddThemeButton()
    {
        if (themeButton != null)
            return;

        themeButton = Img_Theme.GetComponent<Button>();

        if (themeButton == null)
            themeButton = Img_Theme.gameObject.AddComponent<Button>();

        themeButton.targetGraphic = Img_Theme;
        themeButton.onClick.RemoveAllListeners();
        themeButton.onClick.AddListener(OnClickTheme);
    }

    private void RemoveThemeButton()
    {
        if (themeButton == null)
            return;

        themeButton.onClick.RemoveAllListeners();

        Destroy(themeButton);

        themeButton = null;
    }


    private void BuyWithCoin()
    {
        if (isUnlocked || priceType != ThemePriceType.BuyCoin)
            return;

        ResourceManager resource = ResourceManager.Instance;

        if (resource == null)
            return;

        if (resource.Coin < buyCoin)
        {
            showPopupCoin?.Invoke();
            return;
        }

        showQuestion?.Invoke(themeName, BuyCoinConfirmed);
    }

    private void BuyWithGem()
    {
        if (isUnlocked || priceType != ThemePriceType.BuyGem)
            return;

        ResourceManager resource = ResourceManager.Instance;

        if (resource == null)
            return;

        if (resource.Gem < buyGem)
        {
            showPopupGem?.Invoke();
            return;
        }

        showQuestion?.Invoke(themeName, BuyGemConfirmed);
    }


    private void BuyCoinConfirmed()
    {
        ResourceManager resource = ResourceManager.Instance;

        if (resource == null ||
            !resource.SpendResource(ResourceType.Coin, buyCoin))
            return;

        UnlockTheme();
    }

    private void BuyGemConfirmed()
    {
        ResourceManager resource = ResourceManager.Instance;

        if (resource == null ||
            !resource.SpendResource(ResourceType.Gem, buyGem))
            return;

        UnlockTheme();
    }

    private void UnlockTheme()
    {
        isUnlocked = true;

        unlockCallback?.Invoke();

        UpdateUnlockUI();

        onSelect?.Invoke(this);
    }

    private void UpdateUnlockUI()
    {
        Cover.gameObject.SetActive(!isUnlocked);

        Btn_BuyCoin.gameObject.SetActive(
            !isUnlocked && priceType == ThemePriceType.BuyCoin
        );

        Btn_BuyGem.gameObject.SetActive(
            !isUnlocked && priceType == ThemePriceType.BuyGem
        );

        Cost_BuyCoin.gameObject.SetActive(
            !isUnlocked && priceType == ThemePriceType.BuyCoin
        );

        Cost_BuyGem.gameObject.SetActive(
            !isUnlocked && priceType == ThemePriceType.BuyGem
        );

        if (isUnlocked)
            AddThemeButton();
        else
            RemoveThemeButton();
    }

    private void OnClickTheme()
    {
        if (!isUnlocked)
            return;

        onSelect?.Invoke(this);
    }

    public void SetTick(bool active)
    {
        if (Tick != null)
            Tick.gameObject.SetActive(active);
    }




}
