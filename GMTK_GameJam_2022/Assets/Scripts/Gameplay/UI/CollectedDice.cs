using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectedDice : MonoBehaviour
{

    [SerializeField]
    Button selectButton, deSelectButton;

    [SerializeField]
    Image image;

    [SerializeField]
    List<Sprite> diceSprites;

    [SerializeField]
    List<Sprite> selectedDiceSprites;

    public Dice diceType { get; private set; }
    PlayerEntity player;

    public void Init(Dice dice, PlayerEntity player)
    {
        diceType = dice;
        SetSprite(false);
        this.player = player;
    }

    void SetSprite(bool selected)
    {
        image.sprite = diceType switch
        {
            Dice.D4 => selected ? selectedDiceSprites[0] : diceSprites[0],
            Dice.D6 => selected ? selectedDiceSprites[1] : diceSprites[1],
            Dice.D8 => selected ? selectedDiceSprites[2] : diceSprites[2],
            Dice.D10 => selected ? selectedDiceSprites[3] : diceSprites[3],
            Dice.D12 => selected ? selectedDiceSprites[4] : diceSprites[4],
            _ => diceSprites[0]
        };
    }

    public void Select(bool value)
    {
        if (!GameStateManager.Instance.CanRoll)
            return;
        SetSprite(value);
        player.SelectDice(this, value);
        selectButton.gameObject.SetActive(!value);
        deSelectButton.gameObject.SetActive(value);
    }
}
