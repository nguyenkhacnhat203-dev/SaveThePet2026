using UnityEngine;

public class Dog : MonoBehaviour
{
    [Header("Cài đặt lực va chạm")]
    public float maxForce = 2f;

    private Rigidbody2D rb;
    public Collider2D DogCollider { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        DogCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Bee"))
            return;

        if (collision.contacts.Length == 0)
            return;

        Vector2 contactPoint = collision.contacts[0].point;

        Bounds bounds = DogCollider.bounds;
        float midY = bounds.center.y;

        if (contactPoint.y < midY)
        {
            return;
        }

        float topY = bounds.max.y;
        float heightSpan = topY - midY;
        float heightRatio = heightSpan > 0.0001f ? (contactPoint.y - midY) / heightSpan : 0f;

        float actualForce = maxForce * (1f - heightRatio);

        float directionX = (contactPoint.x > bounds.center.x) ? -1f : 1f;

        Vector2 pushForce = new Vector2(directionX * actualForce, 0f);

        rb.AddForceAtPosition(pushForce, contactPoint, ForceMode2D.Impulse);
    }
}