
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
    private bool isWaitingForValidPosition;
    private float currentLength;
    private bool hasDrawn;

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
        if (hasDrawn || isDrawing || isWaitingForValidPosition)
            return;

        Vector3 worldPosition = ScreenToWorld(screenPosition);

        if (IsBlockedByCollider(worldPosition))
        {
            isWaitingForValidPosition = true;
            return;
        }

        BeginDrawing(worldPosition);
    }

    private void BeginDrawing(Vector3 worldPosition)
    {
        isDrawing = true;
        isWaitingForValidPosition = false;
        currentLength = 0f;

        points.Clear();

        UiManager.Instance.ResetHealthBar();

        points.Add(worldPosition);

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, worldPosition);
    }

    public void Draw(Vector3 screenPosition)
    {
        if (hasDrawn)
            return;

        Vector3 worldPosition = ScreenToWorld(screenPosition);

        if (isWaitingForValidPosition)
        {
            if (IsBlockedByCollider(worldPosition))
                return;

            BeginDrawing(worldPosition);
            return;
        }

        if (!isDrawing)
            return;

        Vector3 lastPoint = points[points.Count - 1];

        if (IsPathBlocked(lastPoint, worldPosition))
            return;

        float distance = Vector3.Distance(lastPoint, worldPosition);

        if (distance < minPointDistance)
            return;

        if (currentLength + distance >= maxLength)
        {
            float remaining = maxLength - currentLength;

            if (remaining > 0f)
            {
                Vector3 direction = (worldPosition - lastPoint).normalized;
                worldPosition = lastPoint + direction * remaining;

                if (!IsPathBlocked(lastPoint, worldPosition))
                {
                    points.Add(worldPosition);
                    currentLength = maxLength;
                    UpdateLine();

                    UiManager.Instance.UpdateHealthBar(
                        currentLength,
                        maxLength
                    );
                }
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

    private bool IsBlockedByCollider(Vector2 worldPosition)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPosition);

        return hit != null;
    }

    private bool IsPathBlocked(Vector2 startPoint, Vector2 endPoint)
    {
        if (IsBlockedByCollider(endPoint))
            return true;

        RaycastHit2D[] hits =
            Physics2D.LinecastAll(startPoint, endPoint);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
                return true;
        }

        return false;
    }

    public void StopDrawing()
    {
        if (isWaitingForValidPosition)
        {
            isWaitingForValidPosition = false;
            return;
        }

        if (!isDrawing)
            return;

        isDrawing = false;

        if (points.Count >= 2)
        {
            hasDrawn = true;
            SetupWallPhysics();
        }
    }

    public void CancelDrawing()
    {
        if (hasDrawn)
            return;

        isDrawing = false;
        isWaitingForValidPosition = false;
        currentLength = 0f;

        points.Clear();

        if (lineRenderer != null)
            lineRenderer.positionCount = 0;
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

        rigidbody.collisionDetectionMode =
            CollisionDetectionMode2D.Continuous;

        CreatePolygon(polygonCollider);

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

            leftSide.Add(localPoint + normal * halfWidth);
            rightSide.Add(localPoint - normal * halfWidth);
        }

        List<Vector2> polygon = new List<Vector2>();

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
