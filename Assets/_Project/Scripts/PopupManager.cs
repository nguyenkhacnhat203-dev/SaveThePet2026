using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    [Header("Popup Prefabs")]
    [SerializeField] private PopupWin popupWinPrefab;
    [SerializeField] private PopupFail popupFailPrefab;
    [SerializeField] private PopupNext popupNextPrefab;
    [SerializeField] private PopupTheme popupThemePrefab;

    [Header("Popup Parent")]
    [SerializeField] private Transform popupParent;

    private PopupWin popupWin;
    private PopupFail popupFail;
    private PopupTheme popupTheme;

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

    public void ShowPopupTheme()
    {
        if (popupTheme == null)
            popupTheme = Instantiate(popupThemePrefab, popupParent);

        popupTheme.gameObject.SetActive(true);
    }
}