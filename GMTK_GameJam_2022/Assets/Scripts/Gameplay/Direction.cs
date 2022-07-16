using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Up = 0,
    Right = 1,
    Down = 2,
    Left = 3
}

public static class DirectionSystem
{
    public static Direction RotateRight(Direction originalDirection)
    {
        return (Direction)(((int)originalDirection + 1) % 4);
    }
    public static Direction RotateLeft(Direction originalDirection)
    {
        return (Direction)(((int)originalDirection - 1) % 4);
    }
}
