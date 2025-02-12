using Godot;

namespace SnakeGame
{
	public partial class Cell : Sprite2D
	{
        public static EmptyCell Empty = new EmptyCell();

		private Vector2I _gridPosition;

		
		public Vector2I GridPosition
		{
			get { return _gridPosition; }
			set
			{
				Vector2I newValue = value;
				if (newValue.X < 0)
				{
					newValue.X = 0;
				}

				if (newValue.Y < 0)
				{
					newValue.Y = 0;
				}

				_gridPosition = newValue;
			}
		}

        public ICellOccupier Occupier
		{
			get;
			private set;
		}

		// Palauttaa tiedon siitä, onko solu vapaa vai onko jokin olio siinä.
		public bool IsFree
		{
			get { return Occupier == null || Occupier.Type == CellOccupierType.None; }
		}

		public override void _Ready()
		{
			Occupier = Empty;
		}

		public bool Occupy(ICellOccupier occupier)
		{
			if (!IsFree)
			{
				return false;
			}

			Occupier = occupier;
			return true;
		}


		public void Release()
		{
			Occupier = Empty;
		}

	}
}