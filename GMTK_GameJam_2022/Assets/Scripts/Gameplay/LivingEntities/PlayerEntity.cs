using GMTKJam2022.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerEntity : LivingEntity
{

    List<CollectedDice> collectedDice;

    List<CollectedDice> selectedDice;

    CollectedDice collectedDiceUiPrefab;

    GameUI gameUI;
    int currentRoll;
    Dictionary<Vector2Int, CasinoGrid.GridPathInformation> moveableTiles;
    List<GameObject> spawnedTargetObjects = new List<GameObject>();

    public int CurrentRoll { get; }

    protected override void Awake()
    {
        selectedDice = new List<CollectedDice>();
        collectedDice = new List<CollectedDice>();
        base.Awake();
    }

    public override void Init(CasinoGrid grid)
    {
        base.Init(grid);
        gameUI = FindObjectOfType<GameUI>();
        gameUI.Init(this);
    }

    protected override void Attack(LivingEntity target)
    {
        if(RollDice() > target.RollDice())
        {
            target.TakeDamage();
            AddDice(new List<Dice> { target.DiceType });
        }
    }

    public void SelectDice(CollectedDice dice, bool value)
    {
        if (value && collectedDice.Contains(dice) && !selectedDice.Contains(dice))
            selectedDice.Add(dice);
        else if (value && collectedDice.Contains(dice) && selectedDice.Contains(dice))
            selectedDice.Remove(dice);
    }

    public void StartMovement(bool Reroll)
    {
        if(Reroll)
            RollAndKeep();
        moveableTiles = Grid.FloodFill(GetNearestGridPoint(transform.position), currentRoll);
        SpawnTargetObjects();
        InputManager.Instance.OnGridPointSelected.AddListener(MoveToGridPoint);
        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.TileSelection;
    }

    void ClearSpawnedTargetObjects()
    {
        foreach(GameObject tile in spawnedTargetObjects)
        {
            Destroy(tile);
        }
        spawnedTargetObjects.Clear();
        //moveableTiles.Clear();
    }

    private void SpawnTargetObjects()
    {
        ClearSpawnedTargetObjects();
        foreach (var tile in moveableTiles)
        {
            GameObject newTile = Instantiate(GameManager.Instance.tilePrefab, 
                new Vector3(tile.Key.x + 0.5f, 0.1f, tile.Key.y + 0.5f), Quaternion.identity);
            newTile.name = $"GridTile {tile.Key} - {tile.Value}";
            spawnedTargetObjects.Add(newTile);
        }
    }

    private void MoveToGridPoint(Vector2Int target)
    {
        if (!moveableTiles.Any(m => m.Key.Equals(target)))
            return;
        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.Waiting;
        ClearSpawnedTargetObjects();
        InputManager.Instance.OnGridPointSelected.RemoveListener(MoveToGridPoint);
        List<KeyValuePair<Vector2Int, CasinoGrid.GridPathInformation>> path = 
            new List<KeyValuePair<Vector2Int, CasinoGrid.GridPathInformation>>();

        path.Add(moveableTiles.First(m => m.Key == target));
        currentRoll -= (path[0].Value.Distance);

        int maxIterations = 50;
        while(!path.Any(p => p.Value.Distance == 1) && maxIterations > 0)
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

    public override void OnPathEndReached()
    {
        if (currentRoll > 0)
            StartMovement(false);
        else
            FinishMovement();
    }

    public void RollAndKeep()
    {
        currentRoll = RollDice();
    }

    public void FinishMovement()
    {
        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.ActionSelection;
        currentRoll = 0;
        moveableTiles.Clear();
        InputManager.Instance.OnGridPointSelected.RemoveListener(MoveToGridPoint);
        ClearSpawnedTargetObjects();
    }

    public void AddDice(List<Dice> diceToCollect)
    {
        if (gameUI)
            foreach (Dice dice in diceToCollect)
            {
                collectedDice.Add(gameUI.AddDice(dice, this));
            }
    }

    private void Update()
    {
        Vector2Int targetPosition = new Vector2Int(Mathf.RoundToInt(transform.position.x - 0.5f),
            Mathf.RoundToInt(transform.position.z - 0.5f));
        if (Input.GetKeyDown(KeyCode.UpArrow))
            MoveDownPath(new List<Vector2Int> {
                targetPosition + Vector2Int.right,
                targetPosition + Vector2Int.right + Vector2Int.up,
                targetPosition + Vector2Int.right + Vector2Int.up + Vector2Int.right,
            });
        if (Input.GetKeyDown(KeyCode.DownArrow))
            Move(Direction.Down);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Move(Direction.Right);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Direction.Left);
    }

    public override int RollDice()
    {
        int currentRoll = 0;
        currentRoll += diceType.RollDice();
        string debugText = currentRoll.ToString();
        int prevRoll = 0;
        while(selectedDice.Count > 0)
        {
            prevRoll = currentRoll;
            CollectedDice selectedDie = selectedDice[0];
            currentRoll += selectedDie.diceType.RollDice();
            debugText += " + " + (currentRoll - prevRoll).ToString();
            collectedDice.Remove(selectedDie);
            selectedDice.Remove(selectedDie);
            Destroy(selectedDie.gameObject);
        }
        Debug.Log($"PlayerRoll: {debugText} = {currentRoll}");
        return currentRoll;
    }
}
