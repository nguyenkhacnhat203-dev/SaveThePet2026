using UnityEngine;

public class PopupManager : Singleton<PopupManager>
{
    [SerializeField] private PopupWin popupWinPrefab;
    [SerializeField] private Transform popupParent;

    private PopupWin popupWin;

    public void ShowPopupWin()
    {
        if (popupWin == null)
        {
            popupWin = Instantiate(popupWinPrefab, popupParent);
        }

        popupWin.gameObject.SetActive(true);
    }

  
}