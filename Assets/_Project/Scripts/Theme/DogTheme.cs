
using System.Collections.Generic;
using UnityEngine;

public class DogTheme : MonoBehaviour
{
    [SerializeField] private List<GameObject> costumes = new List<GameObject>();

    private readonly HashSet<GameObject> registeredDogs = new HashSet<GameObject>();

    public IReadOnlyList<GameObject> Costumes => costumes;

    private void Start()
    {
        if (ThemeManager.Instance == null)
            return;

        int selectedIndex = ThemeManager.Instance.GetSelectedTheme(ThemeCategory.Dog);

        if (!ThemeManager.Instance.IsUnlocked(ThemeCategory.Dog, selectedIndex))
        {
            HideAll();
            return;
        }

        SetTheme(selectedIndex);
    }

    public void RegisterDog(GameObject dog)
    {
        if (dog == null)
            return;

        registeredDogs.Add(dog);

        ApplyThemeToDog(dog);
    }

    public void UnregisterDog(GameObject dog)
    {
        if (dog == null)
            return;

        registeredDogs.Remove(dog);
    }

    public void SetTheme(int index)
    {
        if (index < 0 || index >= costumes.Count)
            return;

        for (int i = 0; i < costumes.Count; i++)
        {
            if (costumes[i] != null)
                costumes[i].SetActive(i == index);
        }

        List<GameObject> dogs = new List<GameObject>(registeredDogs);

        foreach (GameObject dog in dogs)
        {
            if (dog == null)
            {
                registeredDogs.Remove(dog);
                continue;
            }

            ApplyThemeToDog(dog, index);
        }
    }

    private void ApplyThemeToDog(GameObject dog)
    {
        if (ThemeManager.Instance == null)
            return;

        int selectedIndex = ThemeManager.Instance.GetSelectedTheme(ThemeCategory.Dog);

        ApplyThemeToDog(dog, selectedIndex);
    }

    private void ApplyThemeToDog(GameObject dog, int selectedIndex)
    {
        if (dog == null || selectedIndex < 0 || selectedIndex >= costumes.Count)
            return;

        Transform[] dogTransforms = dog.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < costumes.Count; i++)
        {
            if (costumes[i] == null)
                continue;

            string costumeName = costumes[i].name;

            for (int j = 0; j < dogTransforms.Length; j++)
            {
                Transform target = dogTransforms[j];

                if (target != dog.transform && target.name == costumeName)
                {
                    target.gameObject.SetActive(i == selectedIndex);
                    break;
                }
            }
        }
    }

    public void HideAll()
    {
        foreach (GameObject costume in costumes)
        {
            if (costume != null)
                costume.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        registeredDogs.Clear();
    }
}
