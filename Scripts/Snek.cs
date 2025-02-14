using Godot;

namespace SnakeGame
{
    public partial class Snek : CharacterBody2D, ICellOccupier
    {
        // Enum on integer, jonka arvot ovat nimetty.
		public enum Direction
		{
			None = 0,
			Up,
			Down,
			Left,
			Right,
		}

		[Export] private float _speed = 1;
        [Export] private Timer _moveTimer = null;

		// TODO: Ei tällaisia kovakoodattuja koordinaatteja!
		private Vector2I _currentPosition = new Vector2I(5, 5);
        // TODO: Onkos tämä nyt hyvää designia?
		// Liikkeen suunta.
		private Direction _currentDirection = Direction.Up;
		// Käyttäjän syötteen suunta.
		private Direction _inputDirection = Direction.None;

		public CellOccupierType Type
		{
			get { return CellOccupierType.Snake; }
		}

        /// <summary>
		/// Snake's position in Grid's coordinates
		/// </summary>
		public Vector2I GridPosition
		{
			get { return _currentPosition; }
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			if (Level.Current.Grid.GetWorldPosition(_currentPosition, out Vector2 worldPosition))
			{
				Position = worldPosition;
			}

            if (_moveTimer == null)
			{
				_moveTimer = GetNode<Timer>("MoveTimer");
				if (_moveTimer == null)
				{
					GD.PrintErr("Move timer cannot be found!");
				}
			}
            
			_moveTimer?.Start();
		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
			Direction direction = ReadInput();
			if (direction != Direction.None)
			{
				_inputDirection = direction;
			}

            if (_moveTimer.IsComplete)
			{
				_inputDirection = ValidateInput(_inputDirection, _currentDirection);
				// Ajastin kävi loppuun, liikuta matoa.
				// Päivitä liikkeen suunta.
				_currentDirection = _inputDirection;
				// Nollataan käyttäjän syötettä kuvaava suunta
				_inputDirection = Direction.None;
				if (_currentDirection != Direction.None)
				{
					Move(_currentDirection);
				}
				// Parametrin nimeäminen auttaa luettavuutta merkittävästi
				_moveTimer.Reset(autoStart: true);
			}
		}

        private Direction ValidateInput(Direction inputDirection, Direction currentDirection)
		{
			switch (currentDirection)
			{
				case Direction.Up:
				case Direction.Down:
					if (inputDirection == Direction.Left || inputDirection == Direction.Right)
					{
						// Mato kääntyy 90 astetta, käännös on laillinen.
						return inputDirection;
					}
					break;
				case Direction.Left:
				case Direction.Right:
					if (inputDirection == Direction.Up || inputDirection == Direction.Down)
					{
						return inputDirection;
					}
					break;
			}
			// Jos käännös ei ollut sallittu, palauta edellinen liikkeen suunta.
			return currentDirection;
		}

		private void Move(Direction direction)
		{
			Vector2I nextPosition = GetNextGridPosition(direction, _currentPosition);
			if (Level.Current.Grid.GetWorldPosition(nextPosition, out Vector2 worldPosition))
			{
                // Liikkuminen sallittu.
				// Tarkista, onko solussa jotain kerättävää.
				Collectable collectable = Level.Current.Grid.GetCollectable(nextPosition);
				if (collectable != null)
				{
					// Kerättävä esine löytyi. Kerää se.
					collectable.Collect(this);
				}

				_currentPosition = nextPosition;
				Position = worldPosition;
				RotationDegrees = GetRotation(direction);
			}
		}

		private void Move(Direction direction, float delta)
		{
			Vector2 directionVector = GetDirectionVector(direction);
			Translate(directionVector * _speed * delta);
		}

		private Vector2I GetNextGridPosition(Direction direction, Vector2I currentPosition)
		{
			switch (direction)
			{
				case Direction.Up: return currentPosition + Vector2I.Up;
				case Direction.Down: return currentPosition + Vector2I.Down;
				case Direction.Right: return currentPosition + Vector2I.Right;
				case Direction.Left: return currentPosition + Vector2I.Left;
				default: return currentPosition; // Mikä tahansa muu case.
			}
		}

		private float GetRotation(Direction movementDirection)
		{
			switch (movementDirection)
			{
				case Direction.Up: return 0;
				case Direction.Down: return 180;
				case Direction.Right: return 90;
				case Direction.Left: return -90;
				default: return 0; // Mikä tahansa muu case.
			}
		}

		private Vector2 GetDirectionVector(Direction direction)
		{
			switch (direction)
			{
				case Direction.Up: return Vector2.Up;
				case Direction.Down: return Vector2.Down;
				case Direction.Right: return Vector2.Right;
				case Direction.Left: return Vector2.Left;
				default: return Vector2.Zero; // Mikä tahansa muu case.
			}
		}

		private Direction ReadInput()
		{
			Direction direction = Direction.None;

			if (Input.IsActionJustPressed(Config.MoveUpAction))
			{
				direction = Direction.Up;
			}
			else if (Input.IsActionJustPressed(Config.MoveDownAction))
			{
				direction = Direction.Down;
			}
			else if (Input.IsActionJustPressed(Config.MoveLeftAction))
			{
				direction = Direction.Left;
			}
			else if (Input.IsActionJustPressed(Config.MoveRightAction))
			{
				direction = Direction.Right;
			}
            else if (Input.IsActionJustPressed(Config.RestartAction))
            {
                Level.Current.ResetGame();
            }

			return direction;
		}
    }
}
