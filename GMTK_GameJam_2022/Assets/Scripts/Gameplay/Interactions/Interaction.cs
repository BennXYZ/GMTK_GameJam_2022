using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction : MonoBehaviour
{
    public bool usesDiceRoll;
    protected Entity entity;

    public virtual bool Interact(LivingEntity interactor, int diceRoll)
    {
        return false;
        //Do Something
    }

    public virtual bool CanBeInteractedWith(LivingEntity interactor)
    {
        return true;
    }

    public void Init(Entity entity)
    {
        this.entity = entity;
    }
}
