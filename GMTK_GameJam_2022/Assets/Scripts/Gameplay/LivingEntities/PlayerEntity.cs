using GMTKJam2022.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerEntity : LivingEntity
{

    List<CollectedDice> collectedDice;

    List<CollectedDice> selectedDice;

    CollectedDice collectedDiceUiPrefab;

    GameUI gameUI;
    List<GameObject> spawnedTargetObjects = new List<GameObject>();
    Dictionary<Entity, GameObject> interactableEntities = new Dictionary<Entity, GameObject>();
    Entity objectToInteract;

    [SerializeField]
    int maxSelectableDice;

    [SerializeField]
    CameraMovement cameraPrefab;

    [SerializeField]
    Canvas canvasPrefabPrefab;

    [SerializeField]
    EventSystem eventSystem;

    protected override void Awake()
    {
        selectedDice = new List<CollectedDice>();
        collectedDice = new List<CollectedDice>();
        base.Awake();
    }

    internal void AfterInteraction(Vector2Int targetPosition)
    {
        currentRoll = 0;
        StartMovement(false);
        ClearInteractables();
        Direction? targetDirection = (targetPosition - GridPosition).ToDirection();
        animator.SetTrigger("Interact");
        if (targetDirection.HasValue)
            direction = targetDirection.Value;
    }

    public bool CanSelectDice()
    {
        return selectedDice != null ? selectedDice.Count < maxSelectableDice : false;
    }

    public override void Init(CasinoGrid grid)
    {
        base.Init(grid);
        GameStateManager.Instance.AssignEntity(this);
        CameraMovement obj = Instantiate(cameraPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<CameraMovement>();
        obj.Init(Grid, this);
        gameUI = Instantiate(canvasPrefabPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<GameUI>();
        Instantiate(eventSystem.gameObject, Vector3.zero, Quaternion.identity);
        gameUI.Init(this);
        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.AskAction;
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
        else if (!value && collectedDice.Contains(dice) && selectedDice.Contains(dice))
            selectedDice.Remove(dice);
    }

    public override void StartMovement(bool Reroll)
    {
        if(Reroll)
            RollAndKeep();
        moveableTiles = Grid.FloodFill(GetNearestGridPoint(transform.position), currentRoll, false);
        SpawnTargetObjects();
        InputManager.Instance.OnGridPointSelected.AddListener(MoveToGridPoint);
        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.MidMovement;
        CheckInteractables();
    }

    public void DeselectAllDice()
    {
        while(selectedDice.Count > 0)
        {
            selectedDice[0].Select(false);
        }
    }

    public void OnStartTurn()
    {
        if (collectedDice.Count == 0)
            AddDice(new List<Dice>() { diceType });
        CheckInteractables(true);
    }

    void CheckInteractables(bool startOfTurn = false)
    {
        ClearInteractables();
        gameUI.ClearInteractables();
        if (currentRoll <= 0 && !startOfTurn)
            return;
        Dictionary<Vector2Int, Entity>  foundInteractableEntities = GameStateManager.Instance.GetInteractableEntities(
            Grid.GetReachableNeighbors(GridPosition));
        foreach (var entity in foundInteractableEntities)
        {
            if (entity.Value.CanBeInteractedWith(this))
            {
                interactableEntities.Add(entity.Value, Instantiate(GameManager.Instance.interactableIndicatorPrefab,
                    entity.Value.transform.position, Quaternion.identity));
                gameUI.CreateInteractable(entity.Value);
            }
        }
    }

    public void OnRollDice()
    {
        if(GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.RollForMovement)
        {
            StartMovement(true);
        }
        else if(GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.RollForInteract)
        {
            if (objectToInteract == null)
            {
                Debug.LogError("No Object To Interact");
                return;
            }
            objectToInteract.Interact(this, RollDice());
            AfterInteraction(objectToInteract.GridPosition);
        }
        else if (GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.RollForDefense)
        {
            
        }
    }

    public void Attack(int rollToBeat)
    {
        
    }

    void ClearInteractables()
    {
        foreach(var entry in interactableEntities)
        {
            Destroy(entry.Value.gameObject);
        }
        interactableEntities.Clear();
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
            CasinoGrid.GridTile? gridTile = Grid.GetTile(tile.Key);
            float heightOffset = gridTile.HasValue ? gridTile.Value.HeightOffset * 0.5f : 0;
            GameObject newTile = Instantiate(GameManager.Instance.tilePrefab, 
                new Vector3(tile.Key.x + 0.5f, heightOffset + 0.1f, tile.Key.y + 0.5f), Quaternion.identity);
            newTile.name = $"GridTile {tile.Key} - {tile.Value}";
            spawnedTargetObjects.Add(newTile);
        }
    }

    public void CheckForInteractions()
    {
        CheckInteractables();
    }

    public void RollForInteraction(Entity entityToInteract)
    {
        objectToInteract = entityToInteract;
        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.RollForInteract;
        ClearSpawnedTargetObjects();
        ClearInteractables();
    }

    protected override void MoveToGridPoint(Vector2Int target)
    {
        if (!moveableTiles.Any(m => m.Key.Equals(target)))
        {
            if(interactableEntities.Any(e => e.Key.GridPosition == target))
            {
                Entity interact = interactableEntities.First(e => e.Key.GridPosition == target).Key;
                if (interact != null)
                {
                    Debug.Log("Interact!");
                    if (interact.InteractionNeedsDice)
                    {
                        RollForInteraction(interact);
                    }
                    else
                    {
                        interact.Interact(this, 0);
                        AfterInteraction(interact.GridPosition);
                    }
                    ClearInteractables();
                }
            }

            return;
        }
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
        base.OnPathEndReached();
        if (currentRoll > 0)
            StartMovement(false);
        CheckInteractables();
        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.MidMovement;
        GameStateManager.Instance.CheckEnemyVision();
    }

    public void EndTurn()
    {
        currentRoll = 0;
        moveableTiles.Clear();
        InputManager.Instance.OnGridPointSelected.RemoveListener(MoveToGridPoint);
        ClearSpawnedTargetObjects();
        ClearInteractables();
        GameStateManager.Instance.DoEnemyTurns();
    }

    public void AddDice(List<Dice> diceToCollect)
    {
        if (gameUI)
            foreach (Dice dice in diceToCollect)
            {
                collectedDice.Add(gameUI.AddDice(dice, this));
            }
    }

    public override int RollDice()
    {
        int currentRoll = 0;
        string debugText = currentRoll.ToString();
        int prevRoll = 0;
        List<KeyValuePair<Dice, int>> result = new List<KeyValuePair<Dice, int>>();
        while(selectedDice.Count > 0)
        {
            prevRoll = currentRoll;
            CollectedDice selectedDie = selectedDice[0];
            currentRoll += selectedDie.diceType.RollDice();
            result.Add(new KeyValuePair<Dice, int>(selectedDie.diceType, currentRoll - prevRoll));
            debugText += " + " + (currentRoll - prevRoll).ToString();
            collectedDice.Remove(selectedDie);
            selectedDice.Remove(selectedDie);
            Destroy(selectedDie.gameObject);
        }
        Debug.Log($"PlayerRoll: {debugText} = {currentRoll}");
        currentRoll = Mathf.Max(currentRoll, 1);
        if(result.Count == 0)
        {
            GameManager.Instance.SpawnDiceRoller(transform, diceType, currentRoll, Vector3.zero);
        }
        else if(result.Count == 1)
        {
            GameManager.Instance.SpawnDiceRoller(transform, result[0].Key, currentRoll, Vector3.zero);
        }
        else
        {
            for (int i = 0; i < result.Count; i++)
            {
                float norm = i / (float)result.Count;
                Vector2 direction = new Vector2(Mathf.Cos(2*Mathf.PI * norm), Mathf.Sin(2 * Mathf.PI * norm)) * 0.5f;
                GameManager.Instance.SpawnDiceRoller(transform, result[i].Key, result[i].Value, new Vector3(direction.x, 0, direction.y));
            }
        }
        return currentRoll;
    }
}
