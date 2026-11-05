using System; 
using System.Collections.Generic;
using UnityEngine;

public class DrawWall : MonoBehaviour
{
    public static event Action OnWallDrawn;

    [SerializeField] private float minPointDistance = 0.05f;
    [SerializeField] private float maxLength = 8f;
    [SerializeField] private float lineWidth = 0.2f;
    [SerializeField] private GameObject wall;

    private Camera mainCamera;
    private LineRenderer lineRenderer;
    private List<Vector3> points = new List<Vector3>();

    private bool isDrawing;
    private float currentLength;

    private void Awake()
    {
        mainCamera = Camera.main;

        lineRenderer = GetComponentInChildren<LineRenderer>();

        if (lineRenderer == null)
        {
            GameObject lineObject = new GameObject("LineRenderer");
            lineObject.transform.SetParent(transform);
            lineObject.transform.localPosition = Vector3.zero;

            lineRenderer = lineObject.AddComponent<LineRenderer>();
        }

        SetupLineRenderer();
    }

    private void SetupLineRenderer()
    {
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.alignment = LineAlignment.View;
        lineRenderer.textureMode = LineTextureMode.Stretch;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.material.color = Color.black;
        lineRenderer.positionCount = 0;
    }

    public void StartDrawing(Vector3 screenPosition)
    {
        if (isDrawing)
            return;

        isDrawing = true;
        currentLength = 0f;

        points.Clear();
        UiManager.Instance.ResetHealthBar();

        Vector3 worldPosition = ScreenToWorld(screenPosition);

        points.Add(worldPosition);

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, worldPosition);
    }

    public void Draw(Vector3 screenPosition)
    {
        if (!isDrawing)
            return;

        Vector3 worldPosition = ScreenToWorld(screenPosition);

        Vector3 lastPoint = points[points.Count - 1];

        float distance = Vector3.Distance(lastPoint, worldPosition);

        if (distance < minPointDistance)
            return;

        if (currentLength + distance >= maxLength)
        {
            float remaining = maxLength - currentLength;

            if (remaining > 0f)
            {
                Vector3 direction =
                    (worldPosition - lastPoint).normalized;

                worldPosition =
                    lastPoint + direction * remaining;

                points.Add(worldPosition);

                currentLength = maxLength;

                UpdateLine();

                UiManager.Instance.UpdateHealthBar(
                    currentLength,
                    maxLength
                );
            }

            return;
        }

        points.Add(worldPosition);

        currentLength += distance;

        UpdateLine();

        UiManager.Instance.UpdateHealthBar(
            currentLength,
            maxLength
        );
    }


    public void StopDrawing()
    {
        if (!isDrawing)
            return;

        isDrawing = false;

        if (points.Count >= 2)
        {
            SetupWallPhysics();
        }
    }

    private void UpdateLine()
    {
        lineRenderer.positionCount = points.Count;

        for (int i = 0; i < points.Count; i++)
        {
            lineRenderer.SetPosition(i, points[i]);
        }
    }

    private void SetupWallPhysics()
    {
        if (wall == null)
            return;

        PolygonCollider2D polygonCollider =
            wall.GetComponent<PolygonCollider2D>();

        if (polygonCollider == null)
            polygonCollider = wall.AddComponent<PolygonCollider2D>();

        Rigidbody2D rigidbody =
            wall.GetComponent<Rigidbody2D>();

        if (rigidbody == null)
            rigidbody = wall.AddComponent<Rigidbody2D>();
        rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        CreatePolygon(polygonCollider);

        // Kích hoạt Action thông báo tường đã vẽ xong
        OnWallDrawn?.Invoke();

        GamePlayManager.Instance.OnWallCreated(wall);
    }

    private void CreatePolygon(PolygonCollider2D polygonCollider)
    {
        List<Vector2> leftSide = new List<Vector2>();
        List<Vector2> rightSide = new List<Vector2>();

        float halfWidth = lineWidth * 0.5f;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 current = points[i];

            Vector2 direction;

            if (i == 0)
            {
                direction =
                    ((Vector2)points[1] - current).normalized;
            }
            else if (i == points.Count - 1)
            {
                direction =
                    (current - (Vector2)points[i - 1]).normalized;
            }
            else
            {
                Vector2 directionA =
                    (current - (Vector2)points[i - 1]).normalized;

                Vector2 directionB =
                    ((Vector2)points[i + 1] - current).normalized;

                direction =
                    (directionA + directionB).normalized;

                if (direction == Vector2.zero)
                    direction = directionB;
            }

            Vector2 normal =
                new Vector2(-direction.y, direction.x);

            Vector2 localPoint =
                wall.transform.InverseTransformPoint(current);

            leftSide.Add(
                localPoint + normal * halfWidth
            );

            rightSide.Add(
                localPoint - normal * halfWidth
            );
        }

        List<Vector2> polygon =
            new List<Vector2>();

        polygon.AddRange(leftSide);

        rightSide.Reverse();

        polygon.AddRange(rightSide);

        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, polygon);
    }

    private Vector3 ScreenToWorld(Vector3 screenPosition)
    {
        Vector3 position = screenPosition;

        position.z =
            Mathf.Abs(mainCamera.transform.position.z);

        Vector3 worldPosition =
            mainCamera.ScreenToWorldPoint(position);

        worldPosition.z = 0f;

        return worldPosition;
    }
}