using UnityEngine;

public class PopupWin : PopupBase
{
    [SerializeField] private GameObject[] stars;

    protected override void Start()
    {
        UpdateStars(UiManager.Instance.CurrentStarCount);
    }

    private void UpdateStars(int starCount)
    {
        if (stars == null)
            return;

        for (int i = 0; i < stars.Length; i++)
        {
            if (stars[i] != null)
                stars[i].SetActive(i < starCount);
        }
    }
}