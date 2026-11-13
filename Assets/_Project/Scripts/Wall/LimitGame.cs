using UnityEngine;

public class LimitGame : MonoBehaviour
{
    [SerializeField] private GameObject limitWallPrefab;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform wallParent;

    private void Start()
    {
        CreateLimitWalls();
    }

    private void CreateLimitWalls()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null || limitWallPrefab == null)
            return;

        float distance = Mathf.Abs(transform.position.z - mainCamera.transform.position.z);

        Vector3 leftPosition = mainCamera.ViewportToWorldPoint(
            new Vector3(0f, 0.5f, distance)
        );

        Vector3 rightPosition = mainCamera.ViewportToWorldPoint(
            new Vector3(1f, 0.5f, distance)
        );

        leftPosition.z = transform.position.z;
        rightPosition.z = transform.position.z;

        Instantiate(limitWallPrefab, leftPosition, Quaternion.identity, wallParent);
        Instantiate(limitWallPrefab, rightPosition, Quaternion.identity, wallParent);
    }
}