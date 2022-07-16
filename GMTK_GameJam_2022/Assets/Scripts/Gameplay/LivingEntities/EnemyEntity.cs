using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : LivingEntity
{
    enum AggroLevel
    {
        Calm = 1,
        Sus = 2,
        Aggro = 3
    }

    [SerializeField]
    bool canMove;

    AggroLevel currentAggrolevel = AggroLevel.Calm;

    public override int RollDice()
    {
        int diceRoll = 0;
        for (int i = 0; i < (int)currentAggrolevel; i++)
        {
            diceRoll += base.RollDice();
        }
        return diceRoll;
    }

    Direction direction;
}
