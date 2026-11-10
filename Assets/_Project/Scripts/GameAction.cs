using System;

public static class GameAction
{
    public static event Action<int, int> OnResourceChanged;

    public static void NotifyResourceChanged(int coin, int gem)
    {
        OnResourceChanged?.Invoke(coin, gem);
    }
}