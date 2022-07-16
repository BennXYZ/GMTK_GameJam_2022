using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    protected Grid grid;

    public virtual void Init(Grid grid)
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
