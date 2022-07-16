using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectDiceInteraction : Interaction
{
    [SerializeField]
    List<Dice> diceToCollect;

    public override void Interact(LivingEntity interactor)
    {
        PlayerEntity player = interactor as PlayerEntity;
        if(player)
        {
            player.AddDice(diceToCollect);
            diceToCollect.Clear();
        }
    }
}
