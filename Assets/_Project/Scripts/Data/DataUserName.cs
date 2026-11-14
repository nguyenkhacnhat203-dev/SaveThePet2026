
using UnityEngine;

public class DataUserName 
{
    public string PlayerName { get; private set; }

    public void SetPlayerName(string playerName)
    {
        PlayerName = playerName;
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();
    }

    public void LoadPlayerName()
    {
        PlayerName = PlayerPrefs.GetString("PlayerName", "Player1");
    }
}
