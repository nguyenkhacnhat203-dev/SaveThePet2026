
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseRanking : MonoBehaviour
{
    public GameObject GoldMedal;
    public GameObject SliverMedal;
    public GameObject CopperMedal;

    public TextMeshProUGUI Count_Score;
    public TextMeshProUGUI Count_Star;
    public TextMeshProUGUI Text_Rank;
    public TextMeshProUGUI Text_Name;

    public Image Avatar;

    public void Setup(LeaderboardPlayerData data, int rank, Sprite avatar)
    {
        if (data == null)
            return;

        if (Text_Name != null)
            Text_Name.text = data.PlayerName;

        if (Text_Rank != null)
            Text_Rank.text = rank.ToString();

        if (Count_Star != null)
            Count_Star.text = data.Star.ToString();

        if (Count_Score != null)
            Count_Score.text = data.Score.ToString();

        if (Avatar != null)
            Avatar.sprite = avatar;

        if (GoldMedal != null)
            GoldMedal.SetActive(rank == 1);

        if (SliverMedal != null)
            SliverMedal.SetActive(rank == 2);

        if (CopperMedal != null)
            CopperMedal.SetActive(rank == 3);
    }
}
