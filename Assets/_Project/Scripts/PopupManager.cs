
using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    [Header("Popup Prefabs")]
    [SerializeField] private PopupWin popupWinPrefab;
    [SerializeField] private PopupFail popupFailPrefab;
    [SerializeField] private PopupNext popupNextPrefab;
    [SerializeField] private PopupTheme popupThemePrefab;
    [SerializeField] private PopupAdd popupAddPrefab;
    [SerializeField] private PopupListLevel popupListLevelPrefab;

    [Header("Popup Parent")]
    [SerializeField] private Transform popupParent;

    private PopupWin popupWin;
    private PopupFail popupFail;
    private PopupTheme popupTheme;
    private PopupAdd popupAdd;
    private PopupListLevel popupListLevel;

    public void ShowPopupWin()
    {
        if (popupWin == null)
            popupWin = Instantiate(popupWinPrefab, popupParent);

        popupWin.gameObject.SetActive(true);
    }

    public void ShowPopupFail()
    {
        if (popupFail == null)
            popupFail = Instantiate(popupFailPrefab, popupParent);

        popupFail.gameObject.SetActive(true);
    }

    public void ShowPopupNextHome()
    {
        PopupNext popup = Instantiate(popupNextPrefab, popupParent);
        popup.SetPopupType(PopupNext.PopupType.Home);
    }

    public void ShowPopupNextGame()
    {
        PopupNext popup = Instantiate(popupNextPrefab, popupParent);
        popup.SetPopupType(PopupNext.PopupType.Game);
    }

    public void ShowPopupNextReplay(int level)
    {
        PopupNext popup = Instantiate(popupNextPrefab, popupParent);
        popup.SetReplayLevel(level);
    }

    public void ShowPopupTheme()
    {
        if (popupTheme == null)
            popupTheme = Instantiate(popupThemePrefab, popupParent);

        popupTheme.gameObject.SetActive(true);
    }

    public void ShowPopupListLevel()
    {
        if (popupListLevel == null)
            popupListLevel = Instantiate(popupListLevelPrefab, popupParent);

        popupListLevel.gameObject.SetActive(true);
    }

    public void ShowPopupAddCoin()
    {
        PopupAdd popup = Instantiate(popupAddPrefab, popupParent);
        popup.SetPopupType(PopupAdd.AddType.Coin);
    }

    public void ShowPopupAddGem()
    {
        PopupAdd popup = Instantiate(popupAddPrefab, popupParent);
        popup.SetPopupType(PopupAdd.AddType.Gem);
    }
}
