using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectDiceInteraction : Interaction
{
    [SerializeField]
    List<Dice> diceToCollect;

    public override bool Interact(LivingEntity interactor, int roll)
    {
        PlayerEntity player = interactor as PlayerEntity;
        if(player)
        {
            player.AddDice(diceToCollect);
            diceToCollect.Clear();
            return true;
        }
        return false;
    }
}
