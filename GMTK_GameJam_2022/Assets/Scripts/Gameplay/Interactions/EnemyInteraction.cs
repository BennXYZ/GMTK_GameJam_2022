using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInteraction : Interaction
{
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
