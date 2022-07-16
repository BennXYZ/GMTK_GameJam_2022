using GMTKJam2022.Gameplay;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyEntity : LivingEntity
{
    enum AggroLevel
    {
        Calm = 1,
        Sus = 2,
        Aggro = 3
    }

    [SerializeField]
    bool canMove;

    [SerializeField]
    AiPath pathToFollow;

    int nextPathTarget;

    AggroLevel currentAggrolevel = AggroLevel.Calm;

    public void DoTurn()
    {
        StartMovement(true);
        MoveToGridPoint(pathToFollow[nextPathTarget]);
    }

    public override void OnPathEndReached()
    {
        if (GetNearestGridPoint(transform.position) == pathToFollow[nextPathTarget])
        {
            nextPathTarget = (nextPathTarget + 1) % pathToFollow.Count;
        }
        if(currentRoll > 0)
        {
            StartMovement(false);
            MoveToGridPoint(pathToFollow[nextPathTarget]);
        }
        else
        {
            GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.ActionSelection;
        }
    }

    public override void StartMovement(bool Reroll)
    {
        if (Reroll)
            RollAndKeep();

        moveableTiles = Grid.FloodFill(GetNearestGridPoint(transform.position), 16);
        while(!moveableTiles.Any(t => t.Key == pathToFollow[nextPathTarget]))
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

    public override int RollDice()
    {
        int diceRoll = 0;
        for (int i = 0; i < (int)currentAggrolevel; i++)
        {
            diceRoll += base.RollDice();
        }
        return diceRoll;
    }
}
