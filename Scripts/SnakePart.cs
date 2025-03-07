using System.Collections.Generic;
using Godot;

namespace SnakeGame
{
    public partial class SnakePart : Node2D, ICellOccupier
    {
        public enum SnakePartType
        {
            None,
            Head,
            Body,
            Tail
        }
        
        public CellOccupierType Type {get { return CellOccupierType.Snake; } }

        [Export] public SnakePartType PartType { get; set; } = SnakePartType.None;
        public Vector2I GridPosition {get; private set;}
        public Grid LevelGrid
        {
            get { return Level.Current.Grid;}
        }
        public static Dictionary<Snek.Direction, int> Directions = new Dictionary<Snek.Direction, int>()
        {
            {Snek.Direction.Up, 0},
            {Snek.Direction.Down, 180},
            {Snek.Direction.Left, 270},
            {Snek.Direction.Right, 90},
        };


        public bool SetPosition(Vector2I gridPosition)
        {
            if (LevelGrid.GetWorldPosition(gridPosition, out Vector2 worldPosition) && 
                LevelGrid.OccupyCell(this, gridPosition))
            {
                // Rotate the right part
                Snek.Direction movingDirection = GetMovingDirection(GridPosition, gridPosition);
                if (movingDirection != Snek.Direction.None)
                {
                    RotationDegrees = Directions[movingDirection];
                }

                GridPosition = gridPosition;
                Position = worldPosition;
                return true;
            }

            return false;
        }

        private Snek.Direction GetMovingDirection(Vector2I originalPosition, Vector2I newPosition)
        {
            if (originalPosition.X < newPosition.X)
            {
                return Snek.Direction.Right;
            }
            if (originalPosition.X > newPosition.X)
            {
                return Snek.Direction.Left;
            }
            if (originalPosition.Y < newPosition.Y)
            {
                return Snek.Direction.Down;
            }
            if (originalPosition.Y > newPosition.Y)
            {
                return Snek.Direction.Up;
            }

            // Don't move
            return Snek.Direction.None;
        }
    }
}