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

        public override string InteractionTitle {
            get => "Finish Level";
        }

        public override string InteractionTip {
            get => "Reach the goal and finish the level.";
        }

        public override bool Interact(LivingEntity interactor, int diceRoll)
        {
            GameStateManager.Instance.CurrentLevel = levelId;
            return true;
        }

        public override bool CanBeInteractedWith(LivingEntity interactor) => interactor is PlayerEntity;
    }
}
