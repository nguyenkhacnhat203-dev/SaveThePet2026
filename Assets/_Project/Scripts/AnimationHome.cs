using System.Collections;
using UnityEngine;

public class AnimationHome : MonoBehaviour
{
    [SerializeField] private Transform dog;
    [SerializeField] private Transform poA;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float height = 2f;

    [SerializeField] private float scaleDuration = 0.25f;

    private void Start()
    {
        StartCoroutine(DogFly());
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

        yield return StartCoroutine(DogScale());
    }

    private IEnumerator DogScale()
    {
        Vector3 originalScale = dog.localScale;

        float duration = 0.5f;
        float timer = 0f;

        while (timer < duration * 2f)
        {
            float wave = Mathf.Sin(timer * Mathf.PI * 4f);

            dog.localScale = new Vector3(
                originalScale.x + wave * 0.08f,
                originalScale.y - wave * 0.04f,
                originalScale.z
            );

            timer += Time.deltaTime;
            yield return null;
        }

        dog.localScale = originalScale;
    }
}