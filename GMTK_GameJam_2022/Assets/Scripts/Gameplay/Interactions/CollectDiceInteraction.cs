using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectDiceInteraction : Interaction
{
    public override string InteractionTitle
    {
        get => "Collect Dice";
    }

    public override string InteractionTip
    {
        get => "You find dice lying around. Interact to collect them.";
    }

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
