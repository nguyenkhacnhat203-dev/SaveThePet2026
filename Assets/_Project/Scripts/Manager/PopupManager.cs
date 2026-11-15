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
    [SerializeField] private PopupProfile popupProfilePrefab;
    [SerializeField] private PopupRanking popupRankingPrefab;
    [SerializeField] private PopupSetiings popupSettingsPrefab;

    [Header("Popup Parent")]
    [SerializeField] private Transform popupParent;

    private PopupWin popupWin;
    private PopupFail popupFail;
    private PopupTheme popupTheme;
    private PopupAdd popupAdd;
    private PopupListLevel popupListLevel;
    private PopupProfile popupProfile;
    private PopupRanking popupRanking;
    private PopupSetiings popupSettings;

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

    public void ShowPopupProfile()
    {
        if (popupProfile == null)
            popupProfile = Instantiate(popupProfilePrefab, popupParent);

        popupProfile.gameObject.SetActive(true);
    }

    public void ShowPopupRanking()
    {
        if (popupRanking == null)
            popupRanking = Instantiate(popupRankingPrefab, popupParent);

        popupRanking.gameObject.SetActive(true);
    }

    public void ShowPopupSettings()
    {
        if (popupSettings == null)
            popupSettings = Instantiate(popupSettingsPrefab, popupParent);

        popupSettings.gameObject.SetActive(true);
    }
}