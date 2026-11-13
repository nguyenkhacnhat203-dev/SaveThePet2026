
using System.Collections.Generic;
using UnityEngine;

public class BeehiveTheme : MonoBehaviour
{
    [Header("Beehive")]
    [SerializeField] private GameObject Beehive;

    [Header("Theme Sprites")]
    [SerializeField] private List<Sprite> Themes = new List<Sprite>();

    private SpriteRenderer beehiveRenderer;

    public IReadOnlyList<Sprite> ThemeSprites => Themes;

    private void Awake()
    {
        FindBeehiveRenderer();
    }

    private void Start()
    {
        if (ThemeManager.Instance == null)
            return;

        int selectedIndex = ThemeManager.Instance.GetSelectedTheme(
            ThemeCategory.Beehive
        );

        if (!ThemeManager.Instance.IsUnlocked(
            ThemeCategory.Beehive,
            selectedIndex))
            return;

        SetTheme(selectedIndex);
    }

    private void FindBeehiveRenderer()
    {
        if (Beehive == null)
            return;

        beehiveRenderer = Beehive.GetComponent<SpriteRenderer>();

        if (beehiveRenderer == null)
            beehiveRenderer = Beehive.GetComponentInChildren<SpriteRenderer>(true);
    }

    public void SetTheme(int index)
    {
        if (index < 0 || index >= Themes.Count)
            return;

        if (Themes[index] == null)
            return;

        if (beehiveRenderer == null)
            FindBeehiveRenderer();

        if (beehiveRenderer == null)
            return;

        beehiveRenderer.sprite = Themes[index];
    }
}
