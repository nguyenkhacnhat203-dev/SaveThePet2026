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
        if (GameManager.Instance.IsPause || GameManager.Instance.IsWin || GameManager.Instance.IsLose)
            return;


        AudioManager.Instance.StopAllSound();
        AudioManager.Instance.PlaySound("TapButton");

        PopupNext popup = Instantiate(popupNextPrefab, popupParent);
        popup.SetPopupType(PopupNext.PopupType.Home);
    }

    public void ShowPopupNextGame()
    {
        AudioManager.Instance.PlaySound("TapButton");

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
        //AudioManager.Instance.PlaySound("TapButton");

        popupTheme.gameObject.SetActive(true);
    }

    public void ShowPopupListLevel()
    {
        if (popupListLevel == null)
            popupListLevel = Instantiate(popupListLevelPrefab, popupParent);
        AudioManager.Instance.PlaySound("TapButton");

        popupListLevel.gameObject.SetActive(true);
    }

    public void ShowPopupAddCoin(GameObject targetEffect)
    {
        AudioManager.Instance.PlaySound("TapButton");

        PopupAdd popup = Instantiate(popupAddPrefab, popupParent);
        popup.SetPopupType(PopupAdd.AddType.Coin, targetEffect);
    }

    public void ShowPopupAddGem(GameObject targetEffect)
    {
        AudioManager.Instance.PlaySound("TapButton");

        PopupAdd popup = Instantiate(popupAddPrefab, popupParent);
        popup.SetPopupType(PopupAdd.AddType.Gem, targetEffect);
    }
    public void ShowPopupProfile()
    {
        if (popupProfile == null)
            popupProfile = Instantiate(popupProfilePrefab, popupParent);
        AudioManager.Instance.PlaySound("TapButton");

        popupProfile.gameObject.SetActive(true);
    }

    public void ShowPopupRanking()
    {
        if (popupRanking == null)
            popupRanking = Instantiate(popupRankingPrefab, popupParent);
        AudioManager.Instance.PlaySound("TapButton");

        popupRanking.gameObject.SetActive(true);
    }

    public void ShowPopupSettings()
    {
        if (popupSettings == null)
            popupSettings = Instantiate(popupSettingsPrefab, popupParent);


        popupSettings.gameObject.SetActive(true);
    }
}