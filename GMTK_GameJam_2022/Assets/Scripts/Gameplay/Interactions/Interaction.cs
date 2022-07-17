using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    public bool usesDiceRoll;
    protected Entity entity;

    public virtual string InteractionTitle { get; }
    public virtual string InteractionTip { get; }

    [SerializeField]
    protected UnityEvent onInteract;

    public virtual bool Interact(LivingEntity interactor, int diceRoll)
    {
        onInteract.Invoke();
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
