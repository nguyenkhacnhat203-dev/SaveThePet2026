
using System.Collections.Generic;
using UnityEngine;

public class BeeManager : Singleton<BeeManager>
{
    [SerializeField] private float randomMoveRadius = 3f;
    [SerializeField] private float randomMoveDuration = 1f;
    [SerializeField] private float randomMoveSpeed = 2f;

    [SerializeField] private float attackSpeed = 5f;

    [SerializeField] private float knockBackDistance = 2f;
    [SerializeField] private float knockBackSpeed = 8f;

    private List<Bee> bees = new List<Bee>();

    private Dictionary<Transform, List<Vector3>> gapsByDog =
        new Dictionary<Transform, List<Vector3>>();

    private float gapScanTimer = 0f;
    private int nextTargetIndex;

    public void RegisterBee(Bee bee, Vector3 spawnPosition, List<Transform> targetDogs)
    {
        if (bee == null || bees.Contains(bee))
            return;

        if (targetDogs == null || targetDogs.Count == 0)
            return;

        Transform targetDog = targetDogs[nextTargetIndex % targetDogs.Count];
        nextTargetIndex++;

        bees.Add(bee);

        bee.Initialize(spawnPosition, randomMoveRadius, targetDog);

        bee.IsGapAttacker = Random.value <= 0.2f;
    }

    public void UnregisterBee(Bee bee)
    {
        if (bee != null)
            bees.Remove(bee);
    }

    public void Tick(float deltaTime)
    {
        gapScanTimer -= deltaTime;

        if (gapScanTimer <= 0)
        {
            ScanForGaps();
            gapScanTimer = 0.2f;
        }

        for (int i = bees.Count - 1; i >= 0; i--)
        {
            Bee bee = bees[i];

            if (bee == null)
            {
                bees.RemoveAt(i);
                continue;
            }

            UpdateBee(bee, deltaTime);
        }
    }

    private void ScanForGaps()
    {
        gapsByDog.Clear();

        HashSet<Transform> scannedDogs = new HashSet<Transform>();

        foreach (Bee bee in bees)
        {
            if (bee == null || bee.TargetDog == null)
                continue;

            Transform dog = bee.TargetDog;

            if (!scannedDogs.Add(dog))
                continue;

            List<Vector3> dogGaps = new List<Vector3>();

            int rayCount = 36;

            for (int i = 0; i < rayCount; i++)
            {
                float angle = i * 10f * Mathf.Deg2Rad;

                Vector2 dir = new Vector2(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle)
                );

                RaycastHit2D[] hits = Physics2D.RaycastAll(
                    dog.position,
                    dir,
                    6f
                );

                bool hitWall = false;

                foreach (var hit in hits)
                {
                    if (hit.collider.CompareTag("Wall"))
                    {
                        hitWall = true;
                        break;
                    }
                }

                if (!hitWall)
                {
                    dogGaps.Add(dog.position + (Vector3)dir * 4f);
                }
            }

            gapsByDog.Add(dog, dogGaps);
        }
    }

    private List<Vector3> GetGapsForDog(Transform dog)
    {
        if (dog == null)
            return null;

        if (gapsByDog.TryGetValue(dog, out List<Vector3> gaps))
            return gaps;

        return null;
    }

    private void UpdateBee(Bee bee, float deltaTime)
    {
        switch (bee.CurrentState)
        {
            case BeeState.KnockBack:
                KnockBackBee(bee, deltaTime);
                break;

            case BeeState.Attack:
                AttackDog(bee, deltaTime);
                break;

            case BeeState.Normal:
                RandomMove(bee, deltaTime);
                bee.AddRandomTime(deltaTime);

                if (bee.RandomTime >= randomMoveDuration)
                {
                    StartAttack(bee);
                }
                break;
        }
    }

    private Vector3 GetValidAttackOffset(Bee bee)
    {
        if (bee.TargetDog == null)
            return Vector3.zero;

        Dog dogScript = bee.TargetDog.GetComponent<Dog>();

        if (dogScript != null && dogScript.DogCollider != null)
        {
            Bounds bounds = dogScript.DogCollider.bounds;

            float thresholdY = bounds.min.y + (bounds.size.y * 0.05f);

            float localX = Random.Range(
                bounds.min.x + 0.1f,
                bounds.max.x - 0.1f
            ) - bee.TargetDog.position.x;

            float localY = Random.Range(
                thresholdY,
                bounds.max.y - 0.1f
            ) - bee.TargetDog.position.y;

            return new Vector3(localX, localY, 0f);
        }

        Vector2 randomOffset = Random.insideUnitCircle * 0.5f;

        return new Vector3(randomOffset.x, randomOffset.y, 0f);
    }

    private void RandomMove(Bee bee, float deltaTime)
    {
        Vector3 direction = bee.RandomTarget - bee.transform.position;

        if (direction.sqrMagnitude <= 0.01f)
        {
            bee.Rb.velocity = Vector2.zero;
            bee.SetRandomTarget(randomMoveRadius);
            return;
        }

        direction.Normalize();

        bee.Rb.velocity = direction * randomMoveSpeed;

        if (direction.x < 0)
        {
            bee.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (direction.x > 0)
        {
            bee.transform.localScale = Vector3.one;
        }

        float angle = Mathf.Atan2(
            direction.y,
            Mathf.Abs(direction.x)
        ) * Mathf.Rad2Deg;

        bee.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void StartAttack(Bee bee)
    {
        if (bee.TargetDog == null)
            return;

        bee.transform.localScale = Vector3.one;
        bee.transform.rotation = Quaternion.identity;

        AssignAttackTarget(bee);

        bee.StartAttack();

        Dog dog = bee.TargetDog.GetComponent<Dog>();

        if (dog != null)
            dog.StartPanic();
    }

    private void StartNextAttack(Bee bee)
    {
        if (bee.TargetDog == null)
            return;

        bee.transform.localScale = Vector3.one;

        AssignAttackTarget(bee);

        bee.StartAttack();
    }

    private void AssignAttackTarget(Bee bee)
    {
        List<Vector3> activeGaps = GetGapsForDog(bee.TargetDog);

        if (bee.IsGapAttacker && activeGaps != null && activeGaps.Count > 0)
        {
            bee.HasReachedGap = false;

            Vector3 bestGap = activeGaps[0];

            float minDistance = float.MaxValue;

            foreach (var gap in activeGaps)
            {
                float dist = Vector3.Distance(
                    bee.transform.position,
                    gap
                );

                if (dist < minDistance)
                {
                    minDistance = dist;
                    bestGap = gap;
                }
            }

            bee.GapTarget = bestGap;
            bee.SetAttackOffset(Vector3.zero);
        }
        else
        {
            bee.SetAttackOffset(GetValidAttackOffset(bee));
        }
    }

    private void AttackDog(Bee bee, float deltaTime)
    {
        if (bee.TargetDog == null)
            return;

        Vector3 currentTargetPosition;

        List<Vector3> activeGaps = GetGapsForDog(bee.TargetDog);

        if (bee.IsGapAttacker &&
            activeGaps != null &&
            activeGaps.Count > 0 &&
            !bee.HasReachedGap)
        {
            currentTargetPosition = bee.GapTarget;

            if (Vector3.Distance(
                bee.transform.position,
                bee.GapTarget
            ) < 0.5f)
            {
                bee.HasReachedGap = true;
            }
        }
        else
        {
            currentTargetPosition =
                bee.TargetDog.position + bee.AttackOffset;
        }

        Vector3 direction =
            currentTargetPosition - bee.transform.position;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
            bee.Rb.velocity = direction * attackSpeed;
        }
        else
        {
            bee.Rb.velocity = Vector2.zero;
        }

        Vector3 dogDirection =
            bee.TargetDog.position - bee.transform.position;

        if (dogDirection.sqrMagnitude > 0.001f)
        {
            RotateBeeToDog(bee, dogDirection.normalized);
        }
    }

    public void BeeHitDog(Bee bee, float offsetMagnitude = 1f)
    {
        if (bee == null || bee.TargetDog == null)
            return;

        Dog dog = bee.TargetDog.GetComponent<Dog>();

        if (dog != null)
            dog.StartPain();

        Vector3 direction =
            bee.transform.position - bee.TargetDog.position;

        if (direction.sqrMagnitude <= 0.001f)
            direction = Vector3.up;

        direction.Normalize();

        Vector3 knockBackTarget =
            bee.transform.position + direction * knockBackDistance;

        float offsetX =
            ((Random.value < 0.5f) ? -1f : 1f) * offsetMagnitude;

        knockBackTarget.x += offsetX;

        bee.SetAttackTarget(knockBackTarget);

        bee.StartKnockBack();
    }

    public void BeeHitWall(
        Bee bee,
        Vector2 hitNormal,
        float horizontalBounce = 1f
    )
    {
        if (bee == null)
            return;

        bee.Rb.position += hitNormal * 0.15f;

        Vector3 bounceDirection = (Vector3)hitNormal.normalized;

        Vector3 newKnockBackTarget =
            bee.transform.position + bounceDirection * knockBackDistance;

        Vector2 tangent = new Vector2(-hitNormal.y, hitNormal.x);

        float sideOffset =
            (Random.value < 0.5f ? -1f : 1f) * horizontalBounce;

        newKnockBackTarget += (Vector3)(tangent * sideOffset);

        bee.SetAttackTarget(newKnockBackTarget);

        bee.StartKnockBack();
    }

    private void KnockBackBee(Bee bee, float deltaTime)
    {
        Vector3 direction =
            bee.AttackTarget - bee.transform.position;

        if (direction.sqrMagnitude <= 0.01f)
        {
            bee.Rb.velocity = Vector2.zero;

            StartNextAttack(bee);

            return;
        }

        direction.Normalize();

        bee.Rb.velocity = direction * knockBackSpeed;

        if (bee.TargetDog != null)
        {
            Vector3 dogDirection =
                bee.TargetDog.position - bee.transform.position;

            if (dogDirection.sqrMagnitude > 0.001f)
            {
                RotateBeeToDog(bee, dogDirection.normalized);
            }
        }
    }

    private void RotateBeeToDog(Bee bee, Vector3 direction)
    {
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;

        bee.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void StopAllBees()
    {
        foreach (Bee bee in bees)
        {
            if (bee == null)
                continue;

            bee.Rb.velocity = Vector2.zero;
            bee.Rb.angularVelocity = 0f;
            bee.Rb.simulated = false;

            bee.StartEndGameMovement();
        }
    }
}
