using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GMTKJam2022.Gameplay.Obstacles
{
    public class ObstacleEntity : Entity
    {
        [SerializeField]
        private Vector2Int[] additionalBlockedTiles;

        private Vector2Int coordinate;

        protected override void Awake()
        {
            Debug.Assert(gameObject.isStatic, "Obstacle is not static", this);
        }

        public override void Init(CasinoGrid grid)
        {
            base.Init(grid);

            coordinate = GetNearestGridPoint(transform.position);
            BlockGridPointByOffset(Vector2Int.zero);

            Array.ForEach(additionalBlockedTiles, BlockGridPointByOffset);
        }

        private void BlockGridPointByOffset(Vector2Int offset)
        {
            CasinoGrid.GridTile? tile = Grid.GetTile(coordinate + offset);
            if (tile.HasValue)
            {
                CasinoGrid.GridTile gridTile = tile.Value;
                gridTile.Type = CasinoGrid.TileType.Blocked;
                Grid.SetTile(coordinate + offset, gridTile);
            }
        }
    }
}
