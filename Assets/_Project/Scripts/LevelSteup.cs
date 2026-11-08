using UnityEngine;

public enum LevelDifficulty
{
    Easy,
    Medium,
    Hard
}

public class LevelSetup : MonoBehaviour
{
    public LevelDifficulty difficulty = LevelDifficulty.Easy;
    public int bonnusCoin = 0;
}