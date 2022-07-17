using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteract : Interaction
{
    public override string InteractionTitle
    {
        get => "Hack Computer";
    }

    public override string InteractionTip
    {
        get => "Roll above the number on the computer to turn of the lasers they are attached to.";
    }

    [SerializeField]
    int numberToBeat = 1;

    [SerializeField]
    LaserEntity laserWall;

    public override bool Interact(LivingEntity interactor, int diceRoll)
    {
        if(diceRoll > numberToBeat)
        {
            //OpenDoor
            laserWall.Open();
            return true;
        }
        else
        {
            //Alert?
            return false;
        }
    }
}
