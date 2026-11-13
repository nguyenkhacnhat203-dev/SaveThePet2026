using UnityEngine;

public abstract class SpecialLevelChallenge : MonoBehaviour
{
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Dog>(out Dog dog))
        {
            Destroy(dog.gameObject);
        }

        if (collision.gameObject.TryGetComponent<Bee>(out Bee bee))
        {
            Destroy(bee.gameObject);
        }
    }

}