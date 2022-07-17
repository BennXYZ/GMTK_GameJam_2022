using GMTKJam2022.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LivingEntity : Entity
{
    [SerializeField]
    protected Dice diceType = Dice.D6;

    [SerializeField]
    protected Direction direction;

    [SerializeField]
    protected Animator animator;

    [SerializeField]
    float movementSpeed;

    [SerializeField]
    float rotationSpeed = 1000;

    [SerializeField]
    LivingEntity debugAttackTarget;

    protected List<Vector2Int> currentPath = new List<Vector2Int>();

    public Dice DiceType { get => diceType; }

    public int CurrentRoll { get; }
    protected int currentRoll;

    public void DebugAttack()
    {
        Attack(debugAttackTarget);
    }

    public override void Init(CasinoGrid grid)
    {
        base.Init(grid);
        AlignToGrid();
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
        if (targetPosition.x <= Grid.Size.x && targetPosition.y <= Grid.Size.y && targetPosition.x >= 0 && targetPosition.y >= 0)
            MoveToPoint(targetPosition);
    }

    protected Dictionary<Vector2Int, CasinoGrid.GridPathInformation> moveableTiles = new Dictionary<Vector2Int, CasinoGrid.GridPathInformation>();

    public virtual void OnPathEndReached()
    {
        if (animator)
            animator.SetBool("IsWalking", false);
    }

    public virtual void StartMovement(bool Reroll)
    {
        if (Reroll)
            RollAndKeep();
        moveableTiles = Grid.FloodFill(GetNearestGridPoint(transform.position), currentRoll, false);
    }

    protected virtual void MoveToGridPoint(Vector2Int target)
    {
        if (animator)
            animator.SetBool("IsWalking", true);
        if (!moveableTiles.Any(m => m.Key.Equals(target)))
            return;
        List<KeyValuePair<Vector2Int, CasinoGrid.GridPathInformation>> path =
            new List<KeyValuePair<Vector2Int, CasinoGrid.GridPathInformation>>();

        path.Add(moveableTiles.First(m => m.Key == target));
        currentRoll -= (path[0].Value.Distance);

        int maxIterations = 50;
        while (!path.Any(p => p.Value.Distance == 1) && maxIterations > 0)
        {
            Vector2Int previousPoint = path[path.Count - 1].Key + path[path.Count - 1].Value.PreviousPoint.ToVector();
            path.Add(moveableTiles.First(m => m.Key == previousPoint));
            maxIterations--;
        }
        if (maxIterations <= 0)
        {
            Debug.LogError("JumpedOutDueToIterations!");
            return;
        }

        path.Sort((a, b) => a.Value.Distance < b.Value.Distance ? -1 : 1);
        MoveDownPath(path.Select(p => p.Key).ToList());
    }

    public void RollAndKeep()
    {
        currentRoll = RollDice();
    }

    public bool LooksAt(Vector2Int gridPosition)
    {
        return GridPosition + direction.ToVector() == gridPosition;
    }

    public void MoveDownPath(List<Vector2Int> path = null)
    {
        if (animator)
            animator.SetBool("IsWalking", true);
        if (path != null)
        {
            currentPath = path;
        }
        if(currentPath.Count > 0)
        {
            MoveToPoint(currentPath[0], false, delegate
            {
                currentPath.RemoveAt(0);
                if(currentPath.Count == 0)
                    OnPathEndReached();
                else
                   MoveDownPath();
            });
        }
    }

    protected virtual void Update()
    {
        Vector3 currentEuler = transform.rotation.eulerAngles;
        Vector3 targetEuler = Vector3.zero;
        switch (direction)
        {
            case Direction.Down:
                targetEuler = Vector3.up * 270; break;
            case Direction.Up:
                targetEuler = Vector3.up * 90; break;
            case Direction.Left:
                targetEuler = Vector3.up * 0; break;
            case Direction.Right:
                targetEuler = Vector3.up * 180; break;
        }
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetEuler), rotationSpeed * Time.deltaTime);
    }

    protected virtual void MoveToPoint(Vector2Int targetPosition, bool instant = false, Action onPositionReached = null)
    {
        CasinoGrid.GridTile? gridTile = Grid.GetTile(targetPosition);
        float heightOffset = gridTile.HasValue ? gridTile.Value.HeightOffset * 0.5f : 0;
        if(!instant)
        {
            Direction? targetDirection = (targetPosition - GridPosition).ToDirection();
            if (targetDirection.HasValue)
                direction = targetDirection.Value;
            else
                Debug.LogError("No Target Direction Achieved");
            StartCoroutine(TransitionToPoint(new Vector3(targetPosition.x + 0.5f, heightOffset, targetPosition.y + 0.5f), onPositionReached));
        }
        else
        {
            transform.position = new Vector3(targetPosition.x + 0.5f, heightOffset, targetPosition.y + 0.5f);
            onPositionReached?.Invoke();
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
                transform.Translate(movement, Space.World);
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
        return diceType.RollDice();
    }
}
