
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : Singleton<FxManager>
{
    public GameObject FireworkLeft;
    public GameObject FireworkRight;

    [SerializeField] private Canvas canvas;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private GameObject gemPrefab;

    [SerializeField] private float spawnRadius = 50f;
    [SerializeField] private float flyDuration = 0.5f;

    [SerializeField] private RectTransform BarCoin;
    [SerializeField] private RectTransform BarGem;

    public RectTransform CoinTarget => BarCoin;
    public RectTransform GemTarget => BarGem;

    public void PlayFireworks(Transform LeftPoint, Transform RightPoint)
    {
        Instantiate(FireworkLeft, LeftPoint.position, Quaternion.identity, transform);
        Instantiate(FireworkRight, RightPoint.position, Quaternion.identity, transform);
    }

    public void PlayCoin(

        Vector3 spawnWorldPos,
        RectTransform target,
        GameObject targetEffect,
        int visualAmount,
        int realAmount,
        Action onComplete = null)
    {
        AudioManager.Instance.PlaySound("ResourceIn");

        StartCoroutine(
            SpawnCurrency(
                spawnWorldPos,
                target,
                coinPrefab,
                visualAmount,
                () =>
                {

                    ResourceManager.Instance.AddResource(
                        ResourceType.Coin,
                        realAmount
                    );
                    AudioManager.Instance.PlaySound("ResourceOut");

                    PlayTargetEffect(targetEffect, onComplete);
                }
            )
        );
    }

    public void PlayGem(
        Vector3 spawnWorldPos,
        RectTransform target,
        GameObject targetEffect,
        int visualAmount,
        int realAmount,
        Action onComplete = null)
    {
        AudioManager.Instance.PlaySound("ResourceIn");

        StartCoroutine(
            SpawnCurrency(
                spawnWorldPos,
                target,
                gemPrefab,
                visualAmount,
                () =>
                {

                    ResourceManager.Instance.AddResource(
                        ResourceType.Gem,
                        realAmount
                    );
                    AudioManager.Instance.PlaySound("ResourceOut");

                    PlayTargetEffect(targetEffect, onComplete);
                }
            )
        );
    }

    private void PlayTargetEffect(GameObject targetEffect, Action onComplete)
    {
        if (targetEffect == null)
        {
            onComplete?.Invoke();
            return;
        }

        Transform target = targetEffect.transform;

        target.DOKill();
        target.localScale = Vector3.one;

        target.DOScale(1.25f, 0.05f)
            .SetEase(Ease.OutQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                target.DOScale(1f, 0.05f)
                    .SetEase(Ease.InQuad)
                    .SetUpdate(true)
                    .OnComplete(() =>
                    {
                        onComplete?.Invoke();
                    });
            });
    }

    private IEnumerator SpawnCurrency(
        Vector3 worldPos,
        RectTransform target,
        GameObject prefab,
        int visualAmount,
        Action onFinished)
    {
        List<GameObject> items = new();

        if (canvas == null || prefab == null || target == null)
        {
            onFinished?.Invoke();
            yield break;
        }

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            onFinished?.Invoke();
            yield break;
        }

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        RectTransform canvasRect = canvas.transform as RectTransform;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPos,
            canvas.worldCamera,
            out Vector2 localPos
        );

        for (int i = 0; i < visualAmount; i++)
        {
            Vector2 randomOffset =
                UnityEngine.Random.insideUnitCircle * spawnRadius;

            GameObject item = Instantiate(prefab, canvas.transform);

            RectTransform rect = item.GetComponent<RectTransform>();

            rect.anchoredPosition = localPos + randomOffset;

            items.Add(item);

            rect.DOAnchorPosY(
                    rect.anchoredPosition.y + 20f,
                    0.3f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine)
                .SetUpdate(true);
        }

        yield return new WaitForSecondsRealtime(0.2f);

        FlyToTarget(items, target, onFinished);
    }

    private void FlyToTarget(
        List<GameObject> items,
        RectTransform target,
        Action onFinished)
    {
        if (items.Count == 0)
        {
            onFinished?.Invoke();
            return;
        }

        int completed = 0;

        foreach (GameObject item in items)
        {
            RectTransform rect = item.GetComponent<RectTransform>();

            rect.DOKill();

            rect.DOMove(target.position, flyDuration)
                .SetEase(Ease.InQuad)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    Destroy(item);

                    completed++;

                    if (completed >= items.Count)
                    {
                        onFinished?.Invoke();
                    }
                });
        }
    }
}
