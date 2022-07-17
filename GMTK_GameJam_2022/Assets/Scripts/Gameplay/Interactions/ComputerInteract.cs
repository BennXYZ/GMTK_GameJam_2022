using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerInteract : Interaction
{
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
