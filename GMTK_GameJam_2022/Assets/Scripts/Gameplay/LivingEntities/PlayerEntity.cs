using GMTKJam2022.Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : LivingEntity
{

    List<CollectedDice> collectedDice;

    List<CollectedDice> selectedDice;

    CollectedDice collectedDiceUiPrefab;

    GameUI gameUI;

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
