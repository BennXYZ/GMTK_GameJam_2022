using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GMTKJam2022.Gameplay
{
    public class CameraMovement : MonoBehaviour
    {
        public CasinoGrid Grid { get; set; }
        public PlayerEntity Player { get; set; }

        public void Init(CasinoGrid grid, PlayerEntity player)
        {
            Grid = grid;
            Player = player;
        }

        private void Update()
        {
            if(Player != null)
            {
                Vector3 clampMin = new Vector3(Grid.Origin.x + 1.5f, 0, Grid.Origin.y + 1.5f);
                Vector3 clampMax = new Vector3(Grid.Origin.x + Grid.Size.x - 1.5f, 0, Grid.Origin.y + Grid.Size.y - 1.5f);

                transform.position = new Vector3(
                    Math.Clamp(Player.transform.position.x, clampMin.x, clampMax.x),
                    Math.Clamp(Player.transform.position.y, clampMin.y, clampMax.y),
                    Math.Clamp(Player.transform.position.z, clampMin.z, clampMax.z)
                    );
            }
        }
    }
}
