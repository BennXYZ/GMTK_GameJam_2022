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
        }
    }

    public override int RollDice()
    {
        int currentRoll = 0;
        selectedDice.Sort();
        currentRoll += RollDice(diceType);
        while(selectedDice.Count > 0)
        {
            currentRoll += RollDice(collectedDice[selectedDice[selectedDice.Count - 1]]);
            collectedDice.RemoveAt(selectedDice[selectedDice.Count - 1]);
        }
        selectedDice.Clear();
        return currentRoll;
    }

    int RollDice(Dice die)
    {
        return Random.Range(0, (int)die) + 1;
    }
}
