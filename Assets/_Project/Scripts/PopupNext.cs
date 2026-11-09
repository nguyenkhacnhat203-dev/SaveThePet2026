
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupNext : PopupBase
{
    public enum PopupType
    {
        Home,
        Game
    }

    private PopupType popupType;
    private Image img;

    public void SetPopupType(PopupType type)
    {
        popupType = type;
        OnShow();
    }

    protected override void Start()
    {
    }

    protected override void OnShow()
    {
        if (Main == null)
            return;

        if (popupType == PopupType.Game)
            GameManager.Instance.ChangeState(GameManager.GameState.Pause);

        img = Main.GetComponent<Image>();

        if (img == null)
            return;

        DOTween.Kill(Main.transform);
        DOTween.Kill(img);


        Main.transform.localScale = Vector3.one;

        Color color = img.color;
        color.a = 0f;
        img.color = color;

        img.DOFade(1f, 0.25f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                img.raycastTarget = false;

                if (popupType == PopupType.Game)
                {
                    LevelManager.Instance.CreateLevel();
                    UiManager.Instance.ShowGameUi();
                }
               else if (popupType == PopupType.Home)
                {
                    UiManager.Instance.ShowHomeUi();
                    LevelManager.Instance.ResetLevel();

                }

                img.DOFade(0f, 0.25f)
                    .SetUpdate(true)
                    .OnComplete(DestroyPopup);
            });
    }

    public override void DestroyPopup()
    {




        Destroy(gameObject);

    }

    private void OnDestroy()
    {

        GameManager.Instance.ChangeState(GameManager.GameState.Playing);

    }
}
