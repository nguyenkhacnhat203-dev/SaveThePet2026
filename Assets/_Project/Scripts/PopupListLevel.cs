
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] private BaseListLevel prefab;

    [Header("Level Group")]
    [SerializeField] private LevelGroup levelGroup;

    private readonly List<BaseListLevel> spawnedLevels = new List<BaseListLevel>();

    private bool isSpawned;

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

    private void OnClickLevel(int level)
    {
        if (level <= 0 || level > levelGroup.Levels.Count)
            return;

        LevelData.SaveLevel(level);

        LevelManager.Instance.CreateLevel();

        DestroyPopup();
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
        Destroy(gameObject);
    }
}
