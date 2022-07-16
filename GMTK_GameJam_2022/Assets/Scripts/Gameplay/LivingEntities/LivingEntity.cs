using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : Entity
{
    [SerializeField]
    protected Dice diceType = Dice.D6;

    [SerializeField]
    float movementSpeed;

    [SerializeField]
    LivingEntity debugAttackTarget;

    List<Vector2Int> currentPath = new List<Vector2Int>();

    public Dice DiceType { get => diceType; }

    public void DebugAttack()
    {
        Attack(debugAttackTarget);
    }

    protected virtual void Awake()
    {
    }

    public void AlignToGrid()
    {
        Vector2Int targetPosition = GetNearestGridPoint(transform.position);
        MoveToPoint(targetPosition, true);
    }

    protected virtual void TargetForAttack(LivingEntity target, bool cancelable)
    {
        //TODO: Add Stuff here where Player can select Dice and then call Attack(target);
    }

    public void Move(Direction direction)
    {
        Vector2Int targetPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x - 0.5f), 
            Mathf.RoundToInt(transform.position.z - 0.5f));
        targetPosition += direction.ToVector();
        if (targetPosition.x <= grid.Size.x && targetPosition.y <= grid.Size.y && targetPosition.x >= 0 && targetPosition.y >= 0)
            MoveToPoint(targetPosition);
    }

    public void MoveDownPath(List<Vector2Int> path = null)
    {
        if(path != null)
        {
            currentPath = path;
        }
        if(currentPath.Count > 0)
        {
            MoveToPoint(currentPath[0], false, delegate
            {
                currentPath.RemoveAt(0);
                MoveDownPath();
            });
        }
    }

    void MoveToPoint(Vector2Int targetPosition, bool instant = false, Action onPositionReached = null)
    {
        //AddSomeMovement
        if (instant)
        {
            transform.position = new Vector3(targetPosition.x + 0.5f, 0, targetPosition.y + 0.5f);
            onPositionReached?.Invoke();
        }
        else
        {
            StartCoroutine(TransitionToPoint(new Vector3(targetPosition.x + 0.5f, 0, targetPosition.y + 0.5f), onPositionReached));
        }
    }

    private IEnumerator TransitionToPoint(Vector3 targetPosition, Action onPositionReached)
    {
        Vector3 movement = targetPosition - transform.position;
        bool pointReached = false;
        while(!pointReached)
        {
            movement = targetPosition - transform.position;
            movement.Normalize();
            movement = movement * Time.deltaTime * movementSpeed;

            if (Mathf.Sign(transform.position.x - targetPosition.x) != Mathf.Sign((transform.position.x + movement.x) - targetPosition.x) ||
                Mathf.Sign(transform.position.z - targetPosition.z) != Mathf.Sign((transform.position.z + movement.z) - targetPosition.z))
            {
                transform.position = targetPosition;
                pointReached = true;
                //Snap To point
            }
            else
            {
                transform.Translate(movement);
                yield return null;
            }
        }
        onPositionReached.Invoke();
    }

    protected virtual void Attack(LivingEntity target)
    {
        if (RollDice() > target.RollDice())
        {
            target.TakeDamage();
        }
    }

    public void TakeDamage()
    {
        Debug.Log($"{name} says Ouch!");
    }

    public virtual int RollDice()
    {
        return DiceSystem.RollDice(diceType);
    }
}
