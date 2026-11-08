using UnityEngine;

public enum ResourceType
{
    Star,
    Gem,
    Coin
}

public class ResourceManager : Singleton<ResourceManager>
{
    public int Star;
    public int Gem;
    public int Coin;
    public DataResource Data { get; private set; }

    private void Start()
    {
        Data = new DataResource();
        Data.LoadData(this);
    }

    public void AddResource(ResourceType type, int amount)
    {
        if (amount <= 0)
            return;

        switch (type)
        {
            case ResourceType.Star:
                Star += amount;
                break;

            case ResourceType.Gem:
                Gem += amount;
                break;

            case ResourceType.Coin:
                Coin += amount;
                break;
        }

        Data.SaveData(this);
    }

    public bool SpendResource(ResourceType type, int amount)
    {
        if (amount <= 0 || GetResource(type) < amount)
            return false;

        switch (type)
        {
            case ResourceType.Star:
                Star -= amount;
                break;

            case ResourceType.Gem:
                Gem -= amount;
                break;

            case ResourceType.Coin:
                Coin -= amount;
                break;

            default:
                return false;
        }

        Data.SaveData(this);
        return true;
    }

    public int GetResource(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.Star:
                return Star;

            case ResourceType.Gem:
                return Gem;

            case ResourceType.Coin:
                return Coin;

            default:
                return 0;
        }
    }
}