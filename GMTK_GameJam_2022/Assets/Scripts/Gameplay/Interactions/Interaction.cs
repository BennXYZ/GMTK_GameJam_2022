using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public bool usesDiceRoll;

    public virtual bool Interact(LivingEntity interactor, int diceRoll)
    {
        return false;
        //Do Something
    }
}
