
using UnityEngine;

public class Dog : MonoBehaviour
{
    public enum DogState
    {
        Normal,
        Panic,
        Satisfied,
        Pain
    }

    public float maxForce = 2f;

    [SerializeField] private float targetGravityScale = 1f;

    private Rigidbody2D rb;

    public Collider2D DogCollider { get; private set; }

    public DogState CurrentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        DogCollider = GetComponent<Collider2D>();

        if (rb != null)
            rb.gravityScale = 0f;

        SetState(DogState.Normal);
    }

    public void SetState(DogState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;
    }

    public void StartPanic()
    {
        if (CurrentState != DogState.Normal)
            return;

        SetState(DogState.Panic);
    }

    public void StartSatisfied()
    {
        if (CurrentState != DogState.Panic)
            return;

        SetState(DogState.Satisfied);
    }

    public void StartPain()
    {
        SetState(DogState.Pain);
    }

    private void OnEnable()
    {
        DrawWall.OnWallDrawn += EnableGravity;
    }

    private void OnDisable()
    {
        DrawWall.OnWallDrawn -= EnableGravity;
    }

    private void EnableGravity()
    {
        if (rb != null)
            rb.gravityScale = targetGravityScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Bee"))
            return;

        StartPain();

        if (collision.contacts.Length == 0)
            return;

        Vector2 contactPoint = collision.contacts[0].point;

        Bounds bounds = DogCollider.bounds;
        float midY = bounds.center.y;

        if (contactPoint.y < midY)
            return;

        float topY = bounds.max.y;
        float heightSpan = topY - midY;

        float heightRatio = heightSpan > 0.0001f
            ? (contactPoint.y - midY) / heightSpan
            : 0f;

        float actualForce = maxForce * (1f - heightRatio);

        float directionX = contactPoint.x > bounds.center.x ? -1f : 1f;

        Vector2 pushForce = new Vector2(directionX * actualForce, 0f);

        if (rb != null)
            rb.AddForceAtPosition(pushForce, contactPoint, ForceMode2D.Impulse);
    }
}