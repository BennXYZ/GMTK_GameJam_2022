using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

    [SerializeField]
    List<DecalProjector> decals;

    [SerializeField]
    Material disabledMaterial;

    public override bool Interact(LivingEntity interactor, int diceRoll)
    {
        if(diceRoll > numberToBeat)
        {
            //OpenDoor
            laserWall.Open();
            foreach(DecalProjector decal in decals)
            {
                decal.material = disabledMaterial;
            }
            onInteract.Invoke();
            return true;
        }
        else
        {
            //Alert?
            return false;
        }
    }
}
