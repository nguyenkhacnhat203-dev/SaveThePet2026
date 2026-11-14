
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupRanking : PopupBase
{
    [Header("Ranking")]
    [SerializeField] private BaseRanking prefab;
    [SerializeField] private Transform Parent;

    [Header("Loading")]
    [SerializeField] private GameObject LoadingIcon;

    [Header("Dog Themes")]
    [SerializeField] private List<Sprite> dogThemes = new List<Sprite>();

    private readonly List<BaseRanking> spawnedRankings = new List<BaseRanking>();

    protected override void Start()
    {
        base.Start();

        if (LoadingIcon != null)
            LoadingIcon.SetActive(true);

        StartCoroutine(LoadRankingWhenReady());
    }

    private IEnumerator LoadRankingWhenReady()
    {
        if (LoadingIcon != null)
            LoadingIcon.SetActive(true);

        while (FirebaseManager.Instance == null ||
               !FirebaseManager.Instance.IsInitialized)
        {
            yield return null;
        }

        FirebaseManager.Instance.GetLeaderboard(SpawnRanking);
    }

    private void SpawnRanking(List<LeaderboardPlayerData> players)
    {
        ClearRanking();

        if (prefab == null || Parent == null)
        {
            Debug.LogError("Chưa gán Prefab hoặc Parent trong PopupRanking!");
            HideLoading();
            return;
        }

        if (players != null && players.Count > 0)
        {
            for (int i = players.Count - 1; i >= 0; i--)
            {
                LeaderboardPlayerData player = players[i];

                int rankIndex = i + 1;

                BaseRanking ranking = Instantiate(prefab, Parent);

                ranking.transform.SetAsFirstSibling();

                Sprite avatar = GetDogTheme(player.DogTheme);

                ranking.Setup(player, rankIndex, avatar);

                spawnedRankings.Insert(0, ranking);
            }
        }

        HideLoading();

        Debug.Log("Đã tạo " + spawnedRankings.Count + " bảng xếp hạng!");
    }

    private Sprite GetDogTheme(int themeIndex)
    {
        if (dogThemes == null ||
            themeIndex < 0 ||
            themeIndex >= dogThemes.Count)
            return null;

        return dogThemes[themeIndex];
    }

    private void ClearRanking()
    {
        for (int i = spawnedRankings.Count - 1; i >= 0; i--)
        {
            if (spawnedRankings[i] != null)
                Destroy(spawnedRankings[i].gameObject);
        }

        spawnedRankings.Clear();
    }

    private void HideLoading()
    {
        if (LoadingIcon != null)
            LoadingIcon.SetActive(false);
    }

    public override void DestroyPopup()
    {
        Destroy(gameObject);
    }
}
