
using UnityEngine;

public class BeeTheme : MonoBehaviour
{
    public GameObject Bee;
    public GameObject Stinger;

    public Sprite BeeYellow;
    public Sprite StingerBeeYellow;

    public Sprite BeeBlue;
    public Sprite StingerBeeBlue;

    public Sprite BeeGreen;
    public Sprite StingerBeeGreen;

    public Sprite BeeOrange;
    public Sprite StingerBeeOrange;

    public Sprite BeeRed;
    public Sprite StingerBeeRed;

    private SpriteRenderer beeRenderer;
    private SpriteRenderer stingerRenderer;

    private void Awake()
    {
        if (Bee != null)
            beeRenderer = Bee.GetComponentInChildren<SpriteRenderer>(true);

        if (Stinger != null)
            stingerRenderer = Stinger.GetComponentInChildren<SpriteRenderer>(true);
    }

    private void Start()
    {
        if (ThemeManager.Instance == null)
            return;

        int selectedIndex = ThemeManager.Instance.GetSelectedTheme(ThemeCategory.Bee);

        if (!ThemeManager.Instance.IsUnlocked(ThemeCategory.Bee, selectedIndex))
            return;

        SetTheme(selectedIndex);
    }

    public void SetTheme(int index)
    {
        Sprite beeSprite = null;
        Sprite stingerSprite = null;

        switch (index)
        {
            case 0:
                beeSprite = BeeYellow;
                stingerSprite = StingerBeeYellow;
                break;

            case 1:
                beeSprite = BeeBlue;
                stingerSprite = StingerBeeBlue;
                break;

            case 2:
                beeSprite = BeeGreen;
                stingerSprite = StingerBeeGreen;
                break;

            case 3:
                beeSprite = BeeOrange;
                stingerSprite = StingerBeeOrange;
                break;

            case 4:
                beeSprite = BeeRed;
                stingerSprite = StingerBeeRed;
                break;

            default:
                return;
        }

        if (beeRenderer != null && beeSprite != null)
            beeRenderer.sprite = beeSprite;

        if (stingerRenderer != null && stingerSprite != null)
            stingerRenderer.sprite = stingerSprite;
    }
}
