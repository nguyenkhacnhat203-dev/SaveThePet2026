
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
    private Button freeButton;

    public void Setup(ThemeItem data, Action<BaseTheme> callback)
    {
        if (data == null)
        {
            return;
        }

        if (Theme == null || Cover == null || Tick == null ||
            Name_Theme == null || Cost_BuyCoin == null ||
            Cost_BuyGem == null || Btn_BuyCoin == null ||
            Btn_BuyGem == null || Img_Theme == null)
        {
            return;
        }

        Theme.sprite = data.Theme;
        Name_Theme.text = data.Name_Theme;

        priceType = data.PriceType;
        onSelect = callback;

        bool isFree = priceType == ThemePriceType.Free;
        bool isBuyCoin = priceType == ThemePriceType.BuyCoin;
        bool isBuyGem = priceType == ThemePriceType.BuyGem;

        Cover.gameObject.SetActive(!isFree);

        Btn_BuyCoin.gameObject.SetActive(isBuyCoin);
        Btn_BuyGem.gameObject.SetActive(isBuyGem);

        Cost_BuyCoin.gameObject.SetActive(isBuyCoin);
        Cost_BuyGem.gameObject.SetActive(isBuyGem);

        if (isBuyCoin)
            Cost_BuyCoin.text = data.BuyCoin.ToString();

        if (isBuyGem)
            Cost_BuyGem.text = data.BuyGem.ToString();

        if (freeButton != null)
        {
            freeButton.onClick.RemoveAllListeners();
            Destroy(freeButton);
            freeButton = null;
        }

        if (isFree)
        {
            freeButton = Img_Theme.GetComponent<Button>();

            if (freeButton == null)
                freeButton = Img_Theme.gameObject.AddComponent<Button>();

            freeButton.targetGraphic = Img_Theme;
            freeButton.onClick.RemoveAllListeners();
            freeButton.onClick.AddListener(OnClickTheme);
        }

        SetTick(false);
    }

    private void OnClickTheme()
    {
        if (priceType != ThemePriceType.Free)
            return;

        onSelect?.Invoke(this);
    }

    public void SetTick(bool active)
    {
        if (Tick != null)
            Tick.gameObject.SetActive(active);
    }
}
