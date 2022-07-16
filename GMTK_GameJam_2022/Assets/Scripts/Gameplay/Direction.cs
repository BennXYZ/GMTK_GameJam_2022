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
    public static Direction RotateRight(this Direction originalDirection)
    {
        return (Direction)(((int)originalDirection + 1) % 4);
    }
    public static Direction RotateLeft(this Direction originalDirection)
    {
        return (Direction)(((int)originalDirection - 1) % 4);
    }

    public static Vector2Int ToVector(this Direction direction)
    {
        return direction switch
        {
            Direction.Up => Vector2Int.up,
            Direction.Right => Vector2Int.right,
            Direction.Down => Vector2Int.down,
            Direction.Left => Vector2Int.left,
            _ => Vector2Int.zero,
        };
    }
}
