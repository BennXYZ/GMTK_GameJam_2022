using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableUiElement : MonoBehaviour
{
    Entity entityToInteract;

    [SerializeField]
    TMP_Text title;

    [SerializeField]
    TMP_Text description;

    public void Init(Entity entityToInteract)
    {
        this.entityToInteract = entityToInteract;
        Interaction interaction = entityToInteract.gameObject.GetComponent<Interaction>();
        title.text = interaction.InteractionTitle;
        description.text = interaction.InteractionTip;
    }

    public void Interact()
    {
        if (!entityToInteract.InteractionNeedsDice)
            entityToInteract.Interact(GameStateManager.Instance.playerEntity, 0);
        else
            GameStateManager.Instance.playerEntity.RollForInteraction(entityToInteract);
    }
}
