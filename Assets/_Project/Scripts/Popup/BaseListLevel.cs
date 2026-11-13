
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BaseListLevel : MonoBehaviour
{
    public Image Star1;
    public Image Star2;
    public Image Star3;

    public GameObject CellStar1;
    public GameObject CellStar2;
    public GameObject CellStar3;

    public Image Cover;
    public Image Level_Img;

    public TextMeshProUGUI Name_Level;

    private int levelIndex;
    private Button levelButton;

    public void Setup(
        LevelItem data,
        int index,
        bool unlocked,
        int starCount,
        Action<int> onClick)
    {
        if (data == null)
            return;

        levelIndex = index;

        Level_Img.sprite = data.LevelImage;
        Name_Level.text = $"Level {levelIndex}";

        UpdateUnlockUI(unlocked);
        UpdateStars(starCount);

        if (unlocked)
            AddLevelButton(onClick);
        else
            RemoveLevelButton();
    }

    private void AddLevelButton(Action<int> onClick)
    {
        if (levelButton == null)
            levelButton = GetComponent<Button>();

        if (levelButton == null)
            levelButton = gameObject.AddComponent<Button>();

        levelButton.onClick.RemoveAllListeners();

        levelButton.onClick.AddListener(() =>
        {
            onClick?.Invoke(levelIndex);
        });
    }

    private void RemoveLevelButton()
    {
        if (levelButton == null)
            levelButton = GetComponent<Button>();

        if (levelButton != null)
        {
            levelButton.onClick.RemoveAllListeners();
            Destroy(levelButton);
            levelButton = null;
        }
    }

    private void UpdateUnlockUI(bool unlocked)
    {
        if (Cover != null)
            Cover.gameObject.SetActive(!unlocked);

        SetCellStarsActive(unlocked);
    }

    private void UpdateStars(int starCount)
    {
        starCount = Mathf.Clamp(starCount, 0, 3);

        if (Star1 != null)
            Star1.gameObject.SetActive(starCount >= 1);

        if (Star2 != null)
            Star2.gameObject.SetActive(starCount >= 2);

        if (Star3 != null)
            Star3.gameObject.SetActive(starCount >= 3);
    }

    private void SetCellStarsActive(bool active)
    {
        if (CellStar1 != null)
            CellStar1.SetActive(active);

        if (CellStar2 != null)
            CellStar2.SetActive(active);

        if (CellStar3 != null)
            CellStar3.SetActive(active);
    }
}
