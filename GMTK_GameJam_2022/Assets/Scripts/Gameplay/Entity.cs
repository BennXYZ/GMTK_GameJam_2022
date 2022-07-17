using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GMTKJam2022.Gameplay;
using System;

public class Entity : MonoBehaviour
{
    public CasinoGrid Grid { get; private set; }
    public bool IsInteractable {
        get => interaction != null;
    }

    Interaction interaction;

    public virtual void Init(CasinoGrid grid)
    {
        Grid = grid;
        GameStateManager.Instance.AssignEntity(this);
        interaction = GetComponent<Interaction>();
        if (interaction != null)
            interaction.Init(this);
    }

    public Vector2Int GridPosition
    {
        get => GetNearestGridPoint(transform.position);
    }
    public bool InteractionNeedsDice {
        get
        {
            if (interaction == null)
                return false;
            return interaction.usesDiceRoll;
        }
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
        if (!Grid)
            return Vector2Int.zero;
        Vector2Int targetPosition = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.z));
        targetPosition.Clamp(Vector2Int.zero, Grid.Size - Vector2Int.one);
        return targetPosition;
    }

    public virtual void Interact(LivingEntity interactor, int diceRoll)
    {
        if (interaction.Interact(interactor, diceRoll))
            interaction = null;
    }

    public bool CanBeInteractedWith(PlayerEntity playerEntity)
    {
        if (interaction == null)
            return false;
        return interaction.CanBeInteractedWith(playerEntity);
    }
}
