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

        NotifyResourceChanged();

        UploadLeaderboard();

        AddResource(ResourceType.Coin, 10000);
        AddResource(ResourceType.Gem, 10000);
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

            default:
                return;
        }

        Data.SaveData(this);
        NotifyResourceChanged();

        if (type == ResourceType.Star)
            UploadLeaderboard();
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
        NotifyResourceChanged();

        if (type == ResourceType.Star)
            UploadLeaderboard();

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

    private void NotifyResourceChanged()
    {
        GameAction.NotifyResourceChanged(Coin, Gem);
    }

    private void UploadLeaderboard()
    {
        if (FirebaseManager.Instance == null)
            return;

        FirebaseManager.Instance.UploadLeaderboard();
    }
}