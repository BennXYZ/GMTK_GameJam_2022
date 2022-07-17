using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GMTKJam2022.Gameplay.Interactions
{
    public class GoalInteraction : Interaction
    {
        [SerializeField]
        private int levelId;

        public override bool Interact(LivingEntity interactor, int diceRoll)
        {
            GameStateManager.Instance.CurrentLevel = levelId;
            return true;
        }

        public override bool CanBeInteractedWith(LivingEntity interactor) => interactor is PlayerEntity;
    }
}
