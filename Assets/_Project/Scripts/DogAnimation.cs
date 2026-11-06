
using UnityEngine;

public class DogAnimation : MonoBehaviour
{
    [SerializeField] private Dog dog;
    [SerializeField] private Animator animator;

    private Dog.DogState previousState;

    private void Awake()
    {
        if (dog == null)
            dog = GetComponent<Dog>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (dog != null)
        {
            previousState = dog.CurrentState;
            UpdateAnimation(previousState);
        }
    }

    private void Update()
    {
        if (dog == null || animator == null)
            return;

        if (previousState == dog.CurrentState)
            return;

        previousState = dog.CurrentState;

        UpdateAnimation(previousState);
    }

    private void UpdateAnimation(Dog.DogState state)
    {
        switch (state)
        {
            case Dog.DogState.Normal:
                animator.Play("Normal");
                break;

            case Dog.DogState.Panic:
                animator.Play("Panic");
                break;

            case Dog.DogState.Satisfied:
                animator.Play("Satisfied");
                break;

            case Dog.DogState.Pain:
                animator.Play("Pain");
                break;
        }
    }
}
