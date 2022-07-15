using UnityEngine;

public enum Dice
{
    D4 = 4,
    D6 = 6,
    D8 = 8,
    D10 = 10,
    D12 = 12,
    D20 = 20
}

public static class DiceSystem
{
    public static int RollDice(Dice die)
    {
        return Random.Range(0, (int)die) + 1;
    }
}