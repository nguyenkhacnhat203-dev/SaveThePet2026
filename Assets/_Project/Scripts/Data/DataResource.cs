using UnityEngine;

[System.Serializable]
public class DataResource
{
    private const string STAR_KEY = "Resource_Star";
    private const string GEM_KEY = "Resource_Gem";
    private const string COIN_KEY = "Resource_Coin";

    public void LoadData(ResourceManager manager)
    {
        manager.Star = PlayerPrefs.GetInt(STAR_KEY, 0);
        manager.Gem = PlayerPrefs.GetInt(GEM_KEY, 0);
        manager.Coin = PlayerPrefs.GetInt(COIN_KEY, 0);
    }

    public void SaveData(ResourceManager manager)
    {
        PlayerPrefs.SetInt(STAR_KEY, manager.Star);
        PlayerPrefs.SetInt(GEM_KEY, manager.Gem);
        PlayerPrefs.SetInt(COIN_KEY, manager.Coin);

        PlayerPrefs.Save();
    }
}