using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTKJam2022.Gameplay;

public class Entity : MonoBehaviour
{
    protected CasinoGrid grid;

    public virtual void Init(CasinoGrid grid)
    {
        this.grid = grid;
    }

    public Vector2Int GetNearestGridPoint(Vector3 position)
    {
        Vector2Int targetPosition = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z));
        targetPosition.Clamp(Vector2Int.zero, grid.Size - Vector2Int.one);
        return targetPosition;
    }
}
