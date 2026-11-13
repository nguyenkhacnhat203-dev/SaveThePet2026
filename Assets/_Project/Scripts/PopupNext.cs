
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopupNext : PopupBase
{
    public enum PopupType
    {
        Home,
        Game,
        Replay
    }

    private PopupType popupType;
    private Image img;
    private int replayLevel;

    public void SetPopupType(PopupType type)
    {
        popupType = type;
        OnShow();
    }

    public void SetReplayLevel(int level)
    {
        replayLevel = level;
        popupType = PopupType.Replay;
        OnShow();
    }

    protected override void Start()
    {
    }

    protected override void OnShow()
    {
        if (Main == null)
            return;

        if (popupType == PopupType.Game || popupType == PopupType.Replay)
            GameManager.Instance.ChangeState(GameManager.GameState.Pause);

        img = Main.GetComponent<Image>();

        if (img == null)
            return;

        DOTween.Kill(Main.transform);
        DOTween.Kill(img);

        Main.transform.localScale = Vector3.one;

        img.raycastTarget = true;

        Color color = img.color;

        if (popupType == PopupType.Replay)
        {
            color.a = 1f;
            img.color = color;

            img.raycastTarget = false;

            ExecuteTransition();
            return;
        }

        color.a = 0f;
        img.color = color;

        img.DOFade(1f, 0.25f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                img.raycastTarget = false;
                ExecuteTransition();
            });
    }

    private void ExecuteTransition()
    {
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
        else if (popupType == PopupType.Replay)
        {
            LevelManager.Instance.CreateLevel(replayLevel);
            UiManager.Instance.ShowGameUi();
        }

        img.DOFade(0f, 0.25f)
            .SetUpdate(true)
            .OnComplete(DestroyPopup);
    }

    public override void DestroyPopup()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ChangeState(GameManager.GameState.Playing);
    }
}
