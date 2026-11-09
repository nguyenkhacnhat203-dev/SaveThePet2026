
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHome : MonoBehaviour
{
    [Header("Dog")]
    [SerializeField] private Transform dog;
    [SerializeField] private Dog dogState;
    [SerializeField] private Transform poA;
    [SerializeField] private Transform PointThrow;

    [Header("Rock")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Transform hive;
    [SerializeField] private float rockDuration = 0.5f;
    [SerializeField] private float rockHeight = 1f;

    [Header("Bees")]
    [SerializeField] private GameObject beePrefab;
    [SerializeField] private Transform[] beeSpawnPoints;
    [SerializeField] private float beeSpeed = 7f;

    [Header("Movement")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private float height = 2f;
    [SerializeField] private float returnSpeed = 5f;

    [Header("Animation")]
    [SerializeField] private float rotateDuration = 0.2f;
    [SerializeField] private float hiveShakeStepDuration = 0.05f;
    [SerializeField] private float hiveLeftAngle = 20f;
    [SerializeField] private float hiveRightAngle = 10f;
    [SerializeField] private int hiveShakeRepeat = 2;
    [SerializeField] private float beeSpawnDelay = 0.2f;

    private Vector3 startPosition;
    private Vector3 originalScale;
    private Quaternion originalRotation;
    private Quaternion originalHiveRotation;

    private Coroutine playLoopCoroutine;

    private GameObject currentRock;
    private List<GameObject> spawnedBees = new List<GameObject>();

    private void Awake()
    {
        if (dogState == null)
            dogState = dog.GetComponent<Dog>();

        startPosition = dog.position;
        originalScale = dog.localScale;
        originalRotation = dog.rotation;
        originalHiveRotation = hive.localRotation;
    }

    private void OnEnable()
    {
        ResetAnimation();

        playLoopCoroutine = StartCoroutine(PlayLoop());
    }

    private void OnDisable()
    {
        if (playLoopCoroutine != null)
        {
            StopCoroutine(playLoopCoroutine);
            playLoopCoroutine = null;
        }

        StopAllCoroutines();

        ClearSpawnedObjects();

        ResetAnimation();
    }

    private void ResetAnimation()
    {
        dog.position = startPosition;
        dog.localScale = originalScale;
        dog.rotation = originalRotation;

        hive.localRotation = originalHiveRotation;

        dogState.SetState(Dog.DogState.Normal);
    }

    private void ClearSpawnedObjects()
    {
        if (currentRock != null)
        {
            Destroy(currentRock);
            currentRock = null;
        }

        for (int i = 0; i < spawnedBees.Count; i++)
        {
            if (spawnedBees[i] != null)
                Destroy(spawnedBees[i]);
        }

        spawnedBees.Clear();
    }

    private IEnumerator PlayLoop()
    {
        while (true)
        {
            yield return StartCoroutine(DogFly());

            yield return StartCoroutine(DogScale());

            yield return StartCoroutine(
                RotateDog(originalRotation * Quaternion.Euler(0f, 0f, -45f))
            );

            Coroutine rockCoroutine = StartCoroutine(ThrowRock());

            yield return StartCoroutine(RotateDog(originalRotation));

            yield return rockCoroutine;

            yield return StartCoroutine(DogReturnAndBeeChase());

            dogState.SetState(Dog.DogState.Normal);
        }
    }

    private IEnumerator DogFly()
    {
        Vector3 startPos = dog.position;
        Vector3 endPos = poA.position;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / duration);

            Vector3 position = Vector3.Lerp(startPos, endPos, t);
            position.y += 4f * height * t * (1f - t);

            dog.position = position;

            yield return null;
        }

        dog.position = endPos;
    }

    private IEnumerator DogScale()
    {
        Vector3 scale = dog.localScale;

        float timer = 0f;
        float scaleDuration = 1f;

        while (timer < scaleDuration)
        {
            float wave = Mathf.Sin(timer * Mathf.PI * 4f);

            dog.localScale = new Vector3(
                scale.x + wave * 0.08f,
                scale.y - wave * 0.04f,
                scale.z
            );

            timer += Time.deltaTime;

            yield return null;
        }

        dog.localScale = scale;
    }

    private IEnumerator RotateDog(Quaternion targetRotation)
    {
        Quaternion startRot = dog.rotation;

        float timer = 0f;

        while (timer < rotateDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / rotateDuration);

            dog.rotation = Quaternion.Slerp(
                startRot,
                targetRotation,
                t
            );

            yield return null;
        }

        dog.rotation = targetRotation;
    }

    private IEnumerator ThrowRock()
    {
        currentRock = Instantiate(
            rockPrefab,
            PointThrow.position,
            PointThrow.rotation,
            PointThrow
        );

        Vector3 startPos = currentRock.transform.position;
        Vector3 endPos = hive.position;

        float timer = 0f;

        while (timer < rockDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / rockDuration);

            Vector3 position = Vector3.Lerp(startPos, endPos, t);
            position.y += 4f * rockHeight * t * (1f - t);

            currentRock.transform.position = position;

            yield return null;
        }

        currentRock.transform.position = endPos;

        Destroy(currentRock);
        currentRock = null;

        yield return StartCoroutine(HiveShake());
    }

    private IEnumerator HiveShake()
    {
        dogState.StartSatisfied();

        for (int i = 0; i < hiveShakeRepeat; i++)
        {
            yield return StartCoroutine(
                RotateHive(
                    originalHiveRotation *
                    Quaternion.Euler(0f, 0f, hiveLeftAngle)
                )
            );

            yield return StartCoroutine(
                RotateHive(originalHiveRotation)
            );

            yield return StartCoroutine(
                RotateHive(
                    originalHiveRotation *
                    Quaternion.Euler(0f, 0f, -hiveRightAngle)
                )
            );

            yield return StartCoroutine(
                RotateHive(originalHiveRotation)
            );
        }

        hive.localRotation = originalHiveRotation;
    }

    private IEnumerator RotateHive(Quaternion targetRotation)
    {
        Quaternion startRot = hive.localRotation;
        float timer = 0f;

        while (timer < hiveShakeStepDuration)
        {
            timer += Time.deltaTime;

            float t = Mathf.Clamp01(timer / hiveShakeStepDuration);

            hive.localRotation = Quaternion.Slerp(
                startRot,
                targetRotation,
                t
            );

            yield return null;
        }

        hive.localRotation = targetRotation;
    }

    private IEnumerator DogReturnAndBeeChase()
    {
        yield return new WaitForSeconds(beeSpawnDelay);

        int beeCount = Mathf.Min(2, beeSpawnPoints.Length);

        Transform[] bees = new Transform[beeCount];

        for (int i = 0; i < beeCount; i++)
        {
            Vector3 spawnPosition = beeSpawnPoints[i] != null
                ? beeSpawnPoints[i].position
                : hive.position;

            GameObject bee = Instantiate(
                beePrefab,
                spawnPosition,
                Quaternion.identity,
                hive
            );

            bees[i] = bee.transform;

            spawnedBees.Add(bee);
        }

        Vector3 flippedScale = originalScale;
        flippedScale.x = -Mathf.Abs(flippedScale.x);

        dog.localScale = flippedScale;

        dogState.StartPanic();

        bool dogReturned = false;
        bool[] beeArrived = new bool[beeCount];

        while (true)
        {
            if (!dogReturned)
            {
                dog.position = Vector3.MoveTowards(
                    dog.position,
                    startPosition,
                    returnSpeed * Time.deltaTime
                );

                if (Vector3.Distance(dog.position, startPosition) < 0.01f)
                {
                    dog.position = startPosition;
                    dogReturned = true;
                }
            }

            bool allBeesArrived = true;

            for (int i = 0; i < bees.Length; i++)
            {
                if (bees[i] == null || beeArrived[i])
                    continue;

                bees[i].position = Vector3.MoveTowards(
                    bees[i].position,
                    dog.position,
                    beeSpeed * Time.deltaTime
                );

                if (Vector3.Distance(bees[i].position, dog.position) < 0.15f)
                {
                    beeArrived[i] = true;
                }
                else
                {
                    allBeesArrived = false;
                }
            }

            if (dogReturned && allBeesArrived)
                break;

            yield return null;
        }

        for (int i = 0; i < bees.Length; i++)
        {
            if (bees[i] != null)
            {
                spawnedBees.Remove(bees[i].gameObject);
                Destroy(bees[i].gameObject);
            }
        }

        dog.localScale = originalScale;
        dog.rotation = originalRotation;
    }
}
