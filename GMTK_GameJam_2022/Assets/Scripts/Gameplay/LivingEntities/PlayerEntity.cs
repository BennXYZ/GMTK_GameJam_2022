using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : LivingEntity
{

    List<Dice> collectedDice;

    List<int> selectedDice;

    protected override void Awake()
    {
        selectedDice = new List<int>();
        collectedDice = new List<Dice>();
        base.Awake();
    }

    protected override void Attack(LivingEntity target)
    {
        if(RollDice() > target.RollDice())
        {
            target.TakeDamage();
            collectedDice.Add(target.DiceType);
        }
    }

    public void AddDice(List<Dice> diceToCollect)
    {
        foreach(Dice dice in diceToCollect)
        {
            collectedDice.Add(dice);
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
        selectedDice.Sort();
        currentRoll += diceType.RollDice();
        while(selectedDice.Count > 0)
        {
            currentRoll += (collectedDice[selectedDice[selectedDice.Count - 1]]).RollDice();
            collectedDice.RemoveAt(selectedDice[selectedDice.Count - 1]);
        }
        selectedDice.Clear();
        return currentRoll;
    }
}
