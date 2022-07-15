using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}

public static class DirectionsSystem
{
    public static Directions RotateRight(Directions originalDirection)
    {
        return (Directions)(((int)originalDirection + 1) % 4);
    }
    public static Directions RotateLeft(Directions originalDirection)
    {
        return (Directions)(((int)originalDirection - 1) % 4);
    }
}
