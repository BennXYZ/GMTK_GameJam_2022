using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : LivingEntity
{

    List<Dice> collectedDice;

    List<int> selectedDice;

    private void Awake()
    {
        selectedDice = new List<int>();
        collectedDice = new List<Dice>();
    }

    protected override void Attack(LivingEntity target)
    {
        if(RollDice() > target.RollDice())
        {
            target.TakeDamage();
            collectedDice.Add(target.DiceType);
        }
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
