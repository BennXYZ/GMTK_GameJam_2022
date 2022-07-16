using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectedDice : MonoBehaviour
{
    [SerializeField]
    TMP_Text diceText;

    public Dice diceType { get; private set; }
    PlayerEntity player;

    public void Init(Dice dice, PlayerEntity player)
    {
        diceType = dice;
        this.player = player;
        diceText.text = dice.ToString();
    }

    public void Select(bool value)
    {
        player.SelectDice(this, value);
    }
}
