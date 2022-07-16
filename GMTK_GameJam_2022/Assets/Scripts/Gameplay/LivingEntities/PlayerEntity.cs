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
            Move(Directions.Down);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Move(Directions.Right);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Move(Directions.Left);
    }

    public override int RollDice()
    {
        int currentRoll = 0;
        selectedDice.Sort();
        currentRoll += DiceSystem.RollDice(diceType);
        while(selectedDice.Count > 0)
        {
            currentRoll += DiceSystem.RollDice(collectedDice[selectedDice[selectedDice.Count - 1]]);
            collectedDice.RemoveAt(selectedDice[selectedDice.Count - 1]);
        }
        selectedDice.Clear();
        return currentRoll;
    }
}
