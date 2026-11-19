
using UnityEngine;

public class TutorialBar : MonoBehaviour
{
    private const string TUTORIAL_KEY = "HAS_SEEN_TUTORIAL";

    private void Start()
    {
        bool hasSeenTutorial = PlayerPrefs.GetInt(TUTORIAL_KEY, 0) == 1;

        gameObject.SetActive(!hasSeenTutorial);
    }

    public void HideTutorial()
    {
        if (!gameObject.activeSelf)
            return;

        gameObject.SetActive(false);

        PlayerPrefs.SetInt(TUTORIAL_KEY, 1);
        PlayerPrefs.Save();
    }
}
