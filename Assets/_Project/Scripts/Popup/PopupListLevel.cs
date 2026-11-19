
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class LevelItem
{
    public Sprite LevelImage;
}

[System.Serializable]
public class LevelGroup
{
    public Transform Parent;
    public List<LevelItem> Levels = new List<LevelItem>();
}

public class PopupListLevel : PopupBase
{
    [SerializeField] private Image blackOverlay;

    [SerializeField] private BaseListLevel prefab;

    [Header("Level Group")]
    [SerializeField] private LevelGroup levelGroup;

    [Header("Pagination")]
    [SerializeField] private Button Btn_Left;
    [SerializeField] private Button Btn_Right;
    [SerializeField] private Image Img_Left;
    [SerializeField] private Image Img_Right;
    [SerializeField] private int levelsPerPage = 10;
    [SerializeField] private byte disabledAlpha = 150;

    [Header("Replay Transition")]
    [SerializeField] private float fadeDuration = 0.25f;

    private readonly List<BaseListLevel> spawnedLevels = new List<BaseListLevel>();

    private int currentPage;
    private int totalPages;

    private bool isSpawned;
    private bool isTransitioning;

    protected override void Start()
    {
        base.Start();

        Btn_Left.onClick.AddListener(PreviousPage);
        Btn_Right.onClick.AddListener(NextPage);

        Spawn();
    }

    public void Spawn()
    {
        if (isSpawned || prefab == null)
            return;

        if (levelGroup == null || levelGroup.Parent == null)
            return;

        levelsPerPage = Mathf.Max(1, levelsPerPage);

        totalPages = Mathf.CeilToInt(
            (float)levelGroup.Levels.Count / levelsPerPage
        );

        currentPage = 0;

        SpawnLevels();
        isSpawned = true;

        UpdateArrowUI();
    }

    private void SpawnLevels()
    {
        ClearLevels();

        if (levelGroup.Levels == null || levelGroup.Levels.Count == 0)
            return;

        int currentLevel = LevelData.LoadLevel();

        int startIndex = currentPage * levelsPerPage;
        int endIndex = Mathf.Min(
            startIndex + levelsPerPage,
            levelGroup.Levels.Count
        );

        for (int i = endIndex - 1; i >= startIndex; i--)
        {
            LevelItem item = levelGroup.Levels[i];

            if (item == null)
                continue;

            int levelIndex = i + 1;
            bool unlocked = levelIndex <= currentLevel;
            int starCount = LevelData.LoadLevelStars(levelIndex);

            BaseListLevel baseLevel = Instantiate(
                prefab,
                levelGroup.Parent
            );

            baseLevel.transform.SetAsFirstSibling();

            baseLevel.Setup(
                item,
                levelIndex,
                unlocked,
                starCount,
                OnClickLevel
            );

            spawnedLevels.Insert(0, baseLevel);
        }
    }

    private void ClearLevels()
    {
        for (int i = 0; i < spawnedLevels.Count; i++)
        {
            if (spawnedLevels[i] != null)
                Destroy(spawnedLevels[i].gameObject);
        }

        spawnedLevels.Clear();
    }

    private void PreviousPage()
    {
        if (isTransitioning || currentPage <= 0)
            return;
        AudioManager.Instance.PlaySound("TapButton");

        currentPage--;

        SpawnLevels();
        UpdateArrowUI();
    }

    private void NextPage()
    {
        if (isTransitioning || currentPage >= totalPages - 1)
            return;
        AudioManager.Instance.PlaySound("TapButton");

        currentPage++;

        SpawnLevels();
        UpdateArrowUI();
    }

    private void UpdateArrowUI()
    {
        bool canGoLeft = currentPage > 0;
        bool canGoRight = currentPage < totalPages - 1;

        UpdateArrowAlpha(Img_Left, canGoLeft);
        UpdateArrowAlpha(Img_Right, canGoRight);

        if (Btn_Left != null)
            Btn_Left.interactable = canGoLeft;

        if (Btn_Right != null)
            Btn_Right.interactable = canGoRight;
    }

    private void UpdateArrowAlpha(Image image, bool active)
    {
        if (image == null)
            return;

        Color color = image.color;
        color.a = active ? 1f : disabledAlpha / 255f;
        image.color = color;
    }

    private void OnClickLevel(int level)
    {
        if (isTransitioning)
            return;

        if (level <= 0 || level > levelGroup.Levels.Count)
            return;

        if (blackOverlay == null)
            return;

        isTransitioning = true;

        AudioManager.Instance.PlaySound("TapButton");

        blackOverlay.gameObject.SetActive(true);

        Color color = blackOverlay.color;
        color.a = 0f;
        blackOverlay.color = color;

        blackOverlay.raycastTarget = true;

        blackOverlay.DOFade(1f, fadeDuration)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                PopupManager.Instance.ShowPopupNextReplay(level);
                Destroy(gameObject);
            });
    }

    public void Refresh()
    {
        ClearLevels();

        isSpawned = false;

        totalPages = Mathf.CeilToInt(
            (float)levelGroup.Levels.Count / Mathf.Max(1, levelsPerPage)
        );

        currentPage = Mathf.Clamp(
            currentPage,
            0,
            Mathf.Max(0, totalPages - 1)
        );

        SpawnLevels();

        isSpawned = true;

        UpdateArrowUI();
    }

    public override void DestroyPopup()
    {
        AudioManager.Instance.PlaySound("TapButton");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (Btn_Left != null)
            Btn_Left.onClick.RemoveListener(PreviousPage);

        if (Btn_Right != null)
            Btn_Right.onClick.RemoveListener(NextPage);

        if (blackOverlay != null)
            DOTween.Kill(blackOverlay);
    }
}
