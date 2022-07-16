using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTKJam2022.Gameplay;

public class Entity : MonoBehaviour
{
    public CasinoGrid Grid { get; private set; }

    [SerializeField]
    Interaction interaction;

    public virtual void Init(CasinoGrid grid)
    {
        Grid = grid;
        GameStateManager.Instance.AssignEntity(this);
    }

    protected virtual void Awake()
    {
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.RemoveEntity(this);
    }

    public Vector2Int GetNearestGridPoint(Vector3 position)
    {
        Vector2Int targetPosition = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
        targetPosition.Clamp(Vector2Int.zero, Grid.Size - Vector2Int.one);
        return targetPosition;
    }

    public virtual void Interact(LivingEntity interactor)
    {
        interaction.Interact(interactor);
    }
}
