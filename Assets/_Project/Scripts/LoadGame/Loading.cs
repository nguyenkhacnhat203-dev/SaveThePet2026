
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Loading : MonoBehaviour
{
    public Image loadingImage;

    [Header("Loading")]
    public float loadTime = 2f;
    public string sceneName = "MainGame";

    [Header("Loading Text")]
    public List<TMP_Text> loadingTexts = new List<TMP_Text>();
    public float dotInterval = 0.15f;

    private bool isLoadingCompleted;
    private bool isSceneLoading;

    private void Start()
    {
        loadingImage.fillAmount = 0f;

        StartCoroutine(LoadProgress());
        StartCoroutine(AnimateLoadingText());
    }

    IEnumerator LoadProgress()
    {
        float timer = 0f;

        while (timer < loadTime)
        {
            timer += Time.deltaTime;

            float progress = timer / loadTime;

            loadingImage.fillAmount = progress;

            yield return null;
        }

        loadingImage.fillAmount = 1f;

        isLoadingCompleted = true;

        ShowAppOpen();
    }

    void ShowAppOpen()
    {
        if (!isLoadingCompleted || isSceneLoading)
            return;

        if (AdsManager.Instance == null)
        {
            LoadNextScene();
            return;
        }

        AdsManager.Instance.ShowAppOpen(LoadNextScene);
    }

    void LoadNextScene()
    {
        if (isSceneLoading)
            return;

        isSceneLoading = true;

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator AnimateLoadingText()
    {
        if (loadingTexts == null || loadingTexts.Count == 0)
            yield break;

        while (true)
        {
            for (int i = 0; i < loadingTexts.Count; i++)
            {
                if (loadingTexts[i] != null)
                    loadingTexts[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < loadingTexts.Count; i++)
            {
                yield return new WaitForSeconds(dotInterval);

                if (loadingTexts[i] != null)
                    loadingTexts[i].gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(dotInterval);

            for (int i = 0; i < loadingTexts.Count; i++)
            {
                if (loadingTexts[i] != null)
                    loadingTexts[i].gameObject.SetActive(false);
            }
        }
    }
}
