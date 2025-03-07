using System.Collections.Generic;
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
        
        // references for snake parts scenes
        [Export] private PackedScene _headScene = null;
        [Export] private PackedScene _bodyScene = null;
        [Export] private PackedScene _tailScene = null;

        // TODO: Onkos tämä nyt hyvää designia?
		// Liikkeen suunta.
		private Direction _currentDirection = Direction.Up;
		// Käyttäjän syötteen suunta.
		private Direction _inputDirection = Direction.None;

        // Snake parts
        private SnakePart _head = null;
        private SnakePart _tail = null;
        private List<SnakePart> _body = new List<SnakePart>();
        private int _length = 3;
		public CellOccupierType Type
		{
			get { return CellOccupierType.Snake; }
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
            if (_moveTimer == null)
			{
				_moveTimer = GetNode<Timer>("MoveTimer");
				if (_moveTimer == null)
				{
					GD.PrintErr("Move timer cannot be found!");
				}
			}
		}

        public void Start()
        {
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

        public bool CreateSnake(Vector2I gridPosition)
        {
            // Add head
            _head = AddBodyPart(SnakePart.SnakePartType.Head, gridPosition);
            if (_head == null)
            {
                return false;
            }

            // loop the body
            int currentLength = 1;
            while (currentLength < _length - 1)
            {
                currentLength++;
                gridPosition.Y++;
                SnakePart body = AddBodyPart(SnakePart.SnakePartType.Body, gridPosition);

                if (body == null)
                {
                    return false;
                }

                _body.Add(body);
            }

            // add tail
            gridPosition.Y++;
            _tail = AddBodyPart(SnakePart.SnakePartType.Tail, gridPosition);

            return _tail != null;
        }

        private SnakePart AddBodyPart(SnakePart.SnakePartType type, Vector2I gridPosition)
        {
            if (!Level.Current.Grid.IsFree(gridPosition))
            {
                // Sends nothing if the snake part can not be created in that coordinate
                return null;
            }

            SnakePart part = null;
            switch(type)
            {
                case SnakePart.SnakePartType.Head:
                    part = _headScene.Instantiate<SnakePart>();
                    break;
                case SnakePart.SnakePartType.Body:
                    part = _bodyScene.Instantiate<SnakePart>();
                    break;
                case SnakePart.SnakePartType.Tail:
                    part = _tailScene.Instantiate<SnakePart>();
                    break;
            }

            if (part != null)
            {
                AddChild(part);
                if(!part.SetPosition(gridPosition))
                {
                    GD.PrintErr("Occupying cell failed");
                }
            }
            
            return part;
        }

        /// <summary>
        /// Releases cells occupied by snake
        /// </summary>
        public void ReleaseCells()
        {
            Level.Current.Grid.ReleaseCell(_head.GridPosition);
            foreach(SnakePart bodyPart in _body)
            {
                Level.Current.Grid.ReleaseCell(bodyPart.GridPosition);
            }
            Level.Current.Grid.ReleaseCell(_tail.GridPosition);
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
			Vector2I nextPosition = GetNextGridPosition(direction, _head.GridPosition);
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

                MoveSnake(nextPosition);
			}
		}

        private void MoveSnake(Vector2I nextPosition)
        {
            ReleaseCells();

            _tail.SetPosition(_body[_body.Count - 1].GridPosition);

            for (int i = _body.Count - 1; i > 0; --i)
            {
                _body[i].SetPosition(_body[i - 1].GridPosition);
            }

            _body[0].SetPosition(_head.GridPosition);

            _head.SetPosition(nextPosition);
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
