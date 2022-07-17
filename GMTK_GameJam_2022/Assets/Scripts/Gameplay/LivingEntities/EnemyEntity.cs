using GMTKJam2022.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyEntity : LivingEntity
{
    public enum AggroLevel
    {
        Calm = 1,
        Sus = 2,
        Aggro = 3
    }

    [SerializeField]
    bool canMove;

    [SerializeField]
    AiPath pathToFollow;

    [SerializeField]
    int pathStartNode;

    int nextPathTarget;

    public AggroLevel CurrentAggrolevel { get {
            return followPlayer > 0 ? AggroLevel.Sus : AggroLevel.Calm;
        } }

    int followPlayer = 0;

    Action turnFinishedAction;

    public override void Init(CasinoGrid grid)
    {
        base.Init(grid);
        nextPathTarget = pathToFollow != null ? pathStartNode % pathToFollow.Count : -1;
    }

    public void DoTurn(Action onTurnFinished)
    {
        turnFinishedAction = onTurnFinished;
        StartMovement(true);
        if (nextPathTarget >= 0 || followPlayer > 0)
            MoveToGridPoint(followPlayer > 0 ? GameStateManager.Instance.playerEntity.GridPosition : pathToFollow[nextPathTarget]);
        else
        {
            if (CheckVisiblePlayer())
                followPlayer = 4;
            OnPathEndReached();
        }
        followPlayer--;
    }

    public override void OnPathEndReached()
    {
        base.OnPathEndReached();
        if(nextPathTarget >= 0)
        {
            if (GetNearestGridPoint(transform.position) == pathToFollow[nextPathTarget])
            {
                nextPathTarget = (nextPathTarget + 1) % pathToFollow.Count;
            }
            if (currentRoll > 0)
            {
                StartMovement(false);
                MoveToGridPoint(pathToFollow[nextPathTarget]);
            }
            else
            {
                turnFinishedAction.Invoke();
                turnFinishedAction = null;
            }
        }
        else
        {
            turnFinishedAction.Invoke();
            turnFinishedAction = null;
        }
    }

    public override void StartMovement(bool Reroll)
    {
        if (Reroll)
            RollAndKeep();

        moveableTiles = Grid.FloodFill(GetNearestGridPoint(transform.position), 16, false);
        if (nextPathTarget >= 0)
            while (!moveableTiles.Any(t => t.Key == pathToFollow[nextPathTarget]))
            {
                nextPathTarget = (nextPathTarget + 1) % pathToFollow.Count;
            }
    }

    protected override void MoveToGridPoint(Vector2Int target)
    {
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
        while(currentRoll < 0)
        {
            path.RemoveAt(0);
            currentRoll++;
        }
        path.Sort((a, b) => a.Value.Distance < b.Value.Distance ? -1 : 1);
        MoveDownPath(path.Select(p => p.Key).ToList());
    }

    protected override void MoveToPoint(Vector2Int targetPosition, bool instant = false, Action onPositionReached = null)
    {
        if (!instant)
        {
            Direction? targetDirection = (targetPosition - GetNearestGridPoint(transform.position)).ToDirection();
            if (targetDirection.HasValue)
                direction = targetDirection.Value;
            else
                Debug.LogError("No Target Direction Achieved");

            if (CheckVisiblePlayer())
            {
                if (followPlayer <= 0)
                {
                    UnityEngine.Debug.Log("PLAYER DETECTED!");
                    currentPath.Clear();
                    currentRoll = 0;
                    OnPathEndReached();
                }
                else
                {
                    if (targetPosition == GameStateManager.Instance.playerEntity.GridPosition)
                    {
                        currentPath.Clear();
                        currentRoll = 0;
                        OnPathEndReached();
                        GameStateManager.Instance.playerEntity.Attack(RollDice());
                    }
                    else
                    {
                        UnityEngine.Debug.Log("PLAYER DETECTED... AGAIN!");
                        base.MoveToPoint(targetPosition, instant, onPositionReached);
                    }
                }
                followPlayer = 3;
            }
            else
            {
                    base.MoveToPoint(targetPosition, instant, onPositionReached);
            }
        }
        else
            base.MoveToPoint(targetPosition, instant, onPositionReached);
    }

    private void OnDrawGizmosSelected()
    {
        if(moveableTiles != null)
        {
            Vector2Int positionToCheck = GridPosition;
            Gizmos.color = Color.cyan;
            while (true)
            {
                positionToCheck += direction.ToVector();
                if (!moveableTiles.Any(t => t.Key == positionToCheck))
                {
                    return;
                }
                Gizmos.DrawWireSphere(new Vector3(positionToCheck.x + 0.5f, 0.5f, positionToCheck.y + 0.5f), 0.2f);
            }
        }
    }

    public void CheckVision()
    {
        if(CheckVisiblePlayer())
        {
            followPlayer = 3;
        }
    }

    private bool CheckVisiblePlayer()
    {
        Vector2Int positionToCheck = GridPosition;
        while (true)
        {
            positionToCheck += direction.ToVector();
            if (!moveableTiles.Any(t => t.Key == positionToCheck))
                return false;
            if (positionToCheck == GameStateManager.Instance.playerEntity.GridPosition)
                return true;
        }
    }

    public override int RollDice()
    {
        int diceRoll = 0;
        for (int i = 0; i < (int)CurrentAggrolevel; i++)
        {
            diceRoll += base.RollDice();
        }
        GameManager.Instance.SpawnDiceRoller(transform, diceType, diceRoll, Vector3.zero);
        return diceRoll;
    }
}
