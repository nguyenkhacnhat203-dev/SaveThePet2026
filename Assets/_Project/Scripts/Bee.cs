
using UnityEngine;
using System.Collections;

public enum BeeState
{
    Normal,
    Attack,
    KnockBack
}

public class Bee : MonoBehaviour
{
    public float RandomTime { get; private set; }
    public BeeState CurrentState { get; private set; }

    public bool IsAttacking => CurrentState == BeeState.Attack;
    public bool IsKnockBack => CurrentState == BeeState.KnockBack;

    public Vector3 RandomTarget { get; private set; }
    public Vector3 AttackTarget { get; private set; }

    public Vector3 AttackOffset { get; private set; }

    public Transform TargetDog { get; private set; }
    public Rigidbody2D Rb { get; private set; }

    public bool IsGapAttacker { get; set; }
    public bool HasReachedGap { get; set; }
    public Vector3 GapTarget { get; set; }

    private Vector3 spawnPosition;
    private Coroutine endGameCoroutine;

    [SerializeField] private float endGameMoveDistance = 1f;
    [SerializeField] private float endGameCycleDuration = 0.2f;
    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();

        if (Rb == null)
            Rb = gameObject.AddComponent<Rigidbody2D>();

        Rb.gravityScale = 0f;
        Rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        Rb.freezeRotation = true;
    }

    public void Initialize(Vector3 spawnPosition, float randomRadius, Transform targetDog)
    {
        this.spawnPosition = spawnPosition;
        TargetDog = targetDog;

        RandomTime = 0f;
        SetState(BeeState.Normal);

        SetRandomTarget(randomRadius);
    }

    public void SetState(BeeState newState)
    {
        CurrentState = newState;
    }

    public void SetRandomTarget(float randomRadius)
    {
        Vector2 randomPosition = Random.insideUnitCircle * randomRadius;

        RandomTarget = spawnPosition + new Vector3(
            randomPosition.x,
            randomPosition.y,
            0f
        );
    }

    public void SetAttackTarget(Vector3 target)
    {
        AttackTarget = target;
    }

    public void SetAttackOffset(Vector3 offset)
    {
        AttackOffset = offset;
    }

    public void AddRandomTime(float deltaTime)
    {
        RandomTime += deltaTime;
    }

    public void StartAttack()
    {
        SetState(BeeState.Attack);
    }

    public void StartKnockBack()
    {
        SetState(BeeState.KnockBack);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CurrentState == BeeState.Normal)
            return;

        if (collision.gameObject.TryGetComponent<Dog>(out Dog dog))
        {
            if (dog.transform != TargetDog)
                return;

            BeeManager.Instance.BeeHitDog(this, 2f);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            if (collision.contacts.Length > 0)
            {
                Vector2 hitNormal = collision.contacts[0].normal;
                BeeManager.Instance.BeeHitWall(this, hitNormal, 3f);
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (CurrentState == BeeState.Normal)
            return;

        if (collision.gameObject.CompareTag("Wall"))
        {
            if (collision.contacts.Length > 0)
            {
                Vector2 hitNormal = collision.contacts[0].normal;
                BeeManager.Instance.BeeHitWall(this, hitNormal, 2f);
            }
        }
    }

    public void StartEndGameMovement()
    {
        if (endGameCoroutine != null)
            StopCoroutine(endGameCoroutine);

        Vector3 startPosition = transform.position;

        endGameCoroutine = StartCoroutine(
            EndGameMovementRoutine(startPosition)
        );
    }

    private IEnumerator EndGameMovementRoutine(Vector3 startPosition)
    {
        float elapsedTime = 0f;

        while (true)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float offsetY = Mathf.Sin(
                elapsedTime / endGameCycleDuration * Mathf.PI * 2f
            ) * endGameMoveDistance;

            transform.position = startPosition + Vector3.up * offsetY;

            yield return null;
        }
    }
}
