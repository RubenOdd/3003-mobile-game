using Godot;
using SnakeGame.UI;

namespace SnakeGame
{
	public partial class Level : Node2D
	{
		// Static on luokan ominaisuus, ei siitä luotujen ominaisuuksien.
		// Kaikki luokasta luodut olio jakavat saman staattisen muuttujan.
		private static Level _current = null;
		public static Level Current
		{
			get { return _current; }
		}

		[Export] private string _snakeScenePath = "res://Scenes/player_snek.tscn";
		[Export] private string _appleScenePath = "res://Scenes/Apple.tscn";
        [Export] private string _nukeScenePath = "res://Scenes/Nuclear.tscn";
        [Export] private TopUIControl _topUIControl = null;

		private PackedScene _snakeScene = null;
		private PackedScene _appleScene = null;
        private PackedScene _nukeScene = null;
		private int _score = 0;
		private Grid _grid = null;
		private Snek _snake = null;
		// Omenoita voi olla olemassa yksi kerrallaan.
		private Apple _apple = null;
        private Nuclear _nuke = null;

		public int Score
		{
			get { return _score; }
			set
			{
				if (value < 0)
				{
					_score = 0;
				}
				else
				{
					_score = value;
				}

				if (_topUIControl != null)
				{
					_topUIControl.SetScore(_score);
				}
			}
		}

		public Grid Grid
		{
			get { return _grid; }
		}

		public Snek Snake
		{
			get { return _snake; }
		}

		// Rakentaja. Käytetään alustamaan olio.
		public Level()
		{
			// _current muuttujaan asetetaan viittaus juuri juotuun Level-olioon.
			// Tälläön Current-propertyn kautta muut oliot pääsevät käsiksi Level-olioon.
			_current = this;
		}

		public override void _Ready()
		{
			_grid = GetNode<Grid>("Grid");
			if (_grid == null)
			{
				GD.PrintErr("Gridiä ei löytynyt Levelin lapsinodeista!");
			}

			ResetGame();
		}

		/// <summary>
		/// Aloittaa uuden pelin.
		/// </summary>
		public void ResetGame()
		{
			DestroySnake();

			// Luo mato
			_snake = CreateSnake();
			AddChild(_snake);

			// Nollaa pisteet
			Score = 0;

			// Luo omena
			ReplaceApple();
            CreateNuke();
		}

		private Snek CreateSnake()
		{
			if (_snakeScene == null)
			{
				_snakeScene = ResourceLoader.Load<PackedScene>(_snakeScenePath);
				if (_snakeScene == null)
				{
					GD.PrintErr("Madon sceneä ei löydy!");
					return null;
				}
			}

            return _snakeScene.Instantiate<Snek>();
		}

        public void DestroySnake()
        {
            if (_snake != null)
            {
                // Vapauta mato Gridistä
                Vector2I snakePosition = _snake.GridPosition;
				Grid.ReleaseCell(snakePosition);

                _snake.QueueFree();
                _snake = null;
            }
        }

		public void ReplaceApple()
		{
			if (_apple != null)
			{
				Grid.ReleaseCell(_apple.GridPosition);

				_apple.QueueFree();
				_apple = null;
			}

			if (_appleScene == null)
			{
				_appleScene = ResourceLoader.Load<PackedScene>(_appleScenePath);
				if (_appleScene == null)
				{
					GD.PrintErr("Can't load apple scene!");
					return;
				}
			}

			_apple = _appleScene.Instantiate<Apple>();
			AddChild(_apple);

			Cell freeCell = Grid.GetRandomFreeCell();
			if (Grid.OccupyCell(_apple, freeCell.GridPosition))
			{
				_apple.SetPosition(freeCell.GridPosition);
			}
		}

        public void CreateNuke()
        {
            if (_nuke != null)
			{
				Grid.ReleaseCell(_nuke.GridPosition);

				_nuke.QueueFree();
				_nuke = null;
			}

			if (_nukeScene == null)
			{
				_nukeScene = ResourceLoader.Load<PackedScene>(_nukeScenePath);
				if (_nukeScene == null)
				{
					GD.PrintErr("Can't load nuke scene!");
					return;
				}
			}

			_nuke = _nukeScene.Instantiate<Nuclear>();
			AddChild(_nuke);

			Cell freeCell = Grid.GetRandomFreeCell();
			if (Grid.OccupyCell(_nuke, freeCell.GridPosition))
			{
				_nuke.SetPosition(freeCell.GridPosition);
			}
        }
	}
}