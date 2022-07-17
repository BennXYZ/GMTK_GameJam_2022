using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

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

    [SerializeField]
    GameObject interactablesUiPrefab;

    [SerializeField]
    Transform interactablesUiParent;

    [SerializeField]
    TMP_Text levelTitle, levelDescription;

    [SerializeField]
    Animator levelInfoAnimator;

    [SerializeField]
    GameObject GameOverUI;

    private static bool staticHasRolledDice, staticHasHitRoll;

    public void EnableRollUi(bool canRoll)
    {
        rollUi.SetActive(canRoll);
    }

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
        ClearInteractables();
        UpdateUI();
        ShowLevelInfo(player.LevelTitle, player.LevelInfo);
    }

    public void ShowLevelInfo(string title, string info)
    {
        levelTitle.text = title;
        levelDescription.text = info;
        levelInfoAnimator.SetTrigger("Rollout");
    }

    public CollectedDice AddDice(Dice diceType, PlayerEntity player)
    {
        CollectedDice instantiatedPrefab = Instantiate(collectedDicePrefab, collectedDiceListParent).GetComponent<CollectedDice>();
        instantiatedPrefab.Init(diceType, player);
        return instantiatedPrefab;
    }

    public void OnMoveButton()
    {
        if(!staticHasRolledDice)
        {
            ShowLevelInfo("Choose Dice", "Pick your dice in the bottom left corner and hit \"Roll\"");
            staticHasRolledDice = true;
        }
        GameStateManager.Instance.CurrentGameState = GameStateManager.GameState.RollForMovement;
    }

    public void RollSelectedDice()
    {
        if (!staticHasHitRoll)
        {
            ShowLevelInfo("Move", "Your roll dictates how much you can move. The red tiles show where you can move");
            staticHasRolledDice = true;
        }
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
            && GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.Waiting
            && GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.GameOver);
        endTurnButton.SetActive(GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.Waiting &&
            GameStateManager.Instance.CurrentGameState != GameStateManager.GameState.EnemyTurn &&
            !GameStateManager.Instance.CanRoll);
        interactablesUiParent.gameObject.SetActive(GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.AskAction ||
            GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.MidMovement);
        GameOverUI.SetActive(GameStateManager.Instance.CurrentGameState == GameStateManager.GameState.GameOver);
    }

    public void ClearInteractables()
    {
        foreach (Transform child in interactablesUiParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateInteractable(Entity entity)
    {
        InteractableUiElement uiElement = Instantiate(interactablesUiPrefab, interactablesUiParent).GetComponent<InteractableUiElement>();
        uiElement.Init(entity);
    }

    public void RestartLevel()
    {
        GameStateManager.Instance.RestartLevel();
    }
}
