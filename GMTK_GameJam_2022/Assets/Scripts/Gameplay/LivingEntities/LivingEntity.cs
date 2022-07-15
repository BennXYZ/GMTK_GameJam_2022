using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    [SerializeField]
    protected Dice diceType = Dice.D6;

    [SerializeField]
    LivingEntity debugAttackTarget;

    public Dice DiceType { get => diceType; }

    public void DebugAttack()
    {
        Attack(debugAttackTarget);
    }

    protected virtual void Attack(LivingEntity target)
    {
        if (RollDice() > target.RollDice())
        {
            target.TakeDamage();
        }
    }

    public void TakeDamage()
    {
        Debug.Log($"{name} says Ouch!");
    }

    public virtual int RollDice()
    {
        return DiceSystem.RollDice(diceType);
    }
}
