
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

    [Header("Replay Transition")]
    [SerializeField] private float fadeDuration = 0.25f;

    private readonly List<BaseListLevel> spawnedLevels = new List<BaseListLevel>();

    private bool isSpawned;
    private bool isTransitioning;

    protected override void Start()
    {
        base.Start();
        Spawn();
    }

    public void Spawn()
    {
        if (isSpawned || prefab == null)
            return;

        if (levelGroup == null || levelGroup.Parent == null)
            return;

        SpawnLevels();
        isSpawned = true;
    }

    private void SpawnLevels()
    {
        if (levelGroup.Levels == null || levelGroup.Levels.Count == 0)
            return;

        int currentLevel = LevelData.LoadLevel();

        for (int i = levelGroup.Levels.Count - 1; i >= 0; i--)
        {
            LevelItem item = levelGroup.Levels[i];

            if (item == null)
                continue;

            int levelIndex = i + 1;
            bool unlocked = levelIndex <= currentLevel;
            int starCount = LevelData.LoadLevelStars(levelIndex);

            BaseListLevel baseLevel = Instantiate(prefab, levelGroup.Parent);

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
        foreach (BaseListLevel level in spawnedLevels)
        {
            if (level != null)
                Destroy(level.gameObject);
        }

        spawnedLevels.Clear();
        isSpawned = false;

        Spawn();
    }

    public override void DestroyPopup()
    {
        AudioManager.Instance.PlaySound("TapButton");

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (blackOverlay != null)
            DOTween.Kill(blackOverlay);
    }
}
