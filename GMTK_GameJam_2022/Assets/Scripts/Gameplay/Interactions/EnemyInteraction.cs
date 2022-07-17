using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInteraction : Interaction
{
    public override string InteractionTitle
    {
        get => "Strike Enemy";
    }

    public override string InteractionTip
    {
        get => "Strike an enemy from behind to knock them out and gather their dice.";
    }

    public override bool CanBeInteractedWith(LivingEntity interactor)
    {
        return !(entity as LivingEntity).LooksAt(interactor.GridPosition);
    }

    public override bool Interact(LivingEntity interactor, int diceRoll)
    {
        (interactor as PlayerEntity).AddDice(new List<Dice> { (entity as EnemyEntity).DiceType });
        Destroy(gameObject);
        return false;
    }
}
