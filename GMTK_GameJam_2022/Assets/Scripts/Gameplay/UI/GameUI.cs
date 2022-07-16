using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    Transform collectedDiceListParent;

    [SerializeField]
    GameObject collectedDicePrefab;

    PlayerEntity player;

    [SerializeField]
    UnityEvent onStartActionSelection = new UnityEvent();
    [SerializeField]
    UnityEvent onFinishActionSelection = new UnityEvent();

    [SerializeField]
    UnityEvent onStartTileSelection = new UnityEvent();
    [SerializeField]
    UnityEvent onFinishTileSelection = new UnityEvent();

    public void TryAddDice()
    {
        int random = Random.Range(2, 7);
        player.AddDice(new List<Dice> { (Dice)(random * 2) });
    }

    public void DebugRoll()
    {
        player.RollDice();
    }

    private void Awake()
    {
        foreach(Transform child in collectedDiceListParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void Start()
    {
        GameStateManager.Instance.AddStartEventsListener(GameStateManager.GameState.ActionSelection, StartActionSelection);
        GameStateManager.Instance.AddFinishEventsListener(GameStateManager.GameState.ActionSelection, FinishActionSelection);
        GameStateManager.Instance.AddStartEventsListener(GameStateManager.GameState.TileSelection, StartTileSelection);
        GameStateManager.Instance.AddFinishEventsListener(GameStateManager.GameState.TileSelection, FinishTileSelection);
    }

    private void OnDestroy()
    {
        GameStateManager.Instance.RemoveStartEventsListener(GameStateManager.GameState.ActionSelection, StartActionSelection);
        GameStateManager.Instance.RemoveFinishEventsListener(GameStateManager.GameState.ActionSelection, FinishActionSelection);
        GameStateManager.Instance.AddStartEventsListener(GameStateManager.GameState.TileSelection, StartTileSelection);
        GameStateManager.Instance.AddFinishEventsListener(GameStateManager.GameState.TileSelection, FinishTileSelection);
    }

    void StartActionSelection()
    {
        onStartActionSelection.Invoke();
    }

    void FinishActionSelection()
    {
        onFinishActionSelection.Invoke();
    }

    void StartTileSelection()
    {
        onStartTileSelection.Invoke();
    }

    void FinishTileSelection()
    {
        onFinishTileSelection.Invoke();
    }

    public void Init(PlayerEntity playerEntity)
    {
        this.player = playerEntity;
    }

    public CollectedDice AddDice(Dice diceType, PlayerEntity player)
    {
        CollectedDice instantiatedPrefab = Instantiate(collectedDicePrefab, collectedDiceListParent).GetComponent<CollectedDice>();
        instantiatedPrefab.Init(diceType, player);
        return instantiatedPrefab;
    }

    public void Move()
    {
        player.StartMovement(true);
        GameManager.Instance.gameStateManager.CurrentGameState = GameStateManager.GameState.TileSelection;
    }

    public void EndMovement()
    {
        player.FinishMovement();
    }
}
