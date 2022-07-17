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
    GameObject rollUi;

    [SerializeField]
    GameObject chooseActionUi, entireUi, endTurnButton;

    public void EnableRollUi(bool canRoll)
    {
        rollUi.SetActive(canRoll);
    }

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

    public void Init(PlayerEntity playerEntity)
    {
        this.player = playerEntity;
        GameStateManager.Instance.AssignUI(this);
        UpdateUI();
    }

    public CollectedDice AddDice(Dice diceType, PlayerEntity player)
    {
        CollectedDice instantiatedPrefab = Instantiate(collectedDicePrefab, collectedDiceListParent).GetComponent<CollectedDice>();
        instantiatedPrefab.Init(diceType, player);
        return instantiatedPrefab;
    }

    public void OnMoveButton()
    {
        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.RollForMovement;
    }

    public void RollSelectedDice()
    {
        player.OnRollDice();
    }

    public void EndMovement()
    {
        player.EndTurn();
    }

    public void UpdateUI()
    {
        rollUi.SetActive(GameStateManager.Instance.CanRoll);
        if (!GameStateManager.Instance.CanRoll)
        {
            player.DeselectAllDice();
        }
        chooseActionUi.SetActive(GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.AskAction);
        entireUi.SetActive(GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.EnemyTurn 
            && GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.Waiting);
        endTurnButton.SetActive(GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.Waiting &&
            GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.EnemyTurn &&
            !GameStateManager.Instance.CanRoll);
    }
}
