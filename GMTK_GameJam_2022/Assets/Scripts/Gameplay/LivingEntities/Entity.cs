using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTKJam2022.Gameplay;

public class Entity : MonoBehaviour
{
    public CasinoGrid Grid { get; private set; }

    public virtual void Init(CasinoGrid grid)
    {
        Grid = grid;
    }

    public Vector2Int GetNearestGridPoint(Vector3 position)
    {
        Vector2Int targetPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        targetPosition.Clamp(Vector2Int.zero, Grid.Size - Vector2Int.one);
        return targetPosition;
    }
}
