using System.Collections.Generic;
using Godot;

namespace SnakeGame
{
	public partial class Grid : Node2D
	{
		[Export] private string _cellScenePath = "res://Scenes/cell.tscn";
		[Export] private int _width = 0;
		[Export] private int _height = 0;

		// Vector2I on integeriä kullekin koordinaatille yksikkönä käyttävä vektorityyppi.
		[Export] private Vector2I _cellSize = Vector2I.Zero;

		// TODO: Kirjoita julkinen property, joka mahdollistaa _width jäsenmuuttujan lukemisen,
		// muttei asettamista. Anna propertyn nimeksi Width.
		// TODO: Kirjoita vastaava property _height jäsenmuuttujalle. Propertyn nimi tulee olla Height.

        public int Width => _width;
        public int Height => _height;

		// Tähän 2-uloitteiseen taulukkoon on tallennettu gridin solut. Alussa taulukkoa ei ole, vaan
		// muuttujassa on tyhjä viittaus (null). Taulukko pitää luoda pelin alussa (esim. _Ready-metodissa).
		private Cell[,] _cells = null;

		public override void _Ready()
		{
			// TODO: Alusta _cells taulukko
            _cells = new Cell[_width, _height];

			// Laske se piste, josta taulukon rakentaminen aloitetaan. Koska 1. solu luodaan gridin vasempaan
			// yläkulmaan, on meidän laskettava sitä koordinaattia vastaava piste. Oletetaan Gridin pivot-pisteen
			// olevan kameran keskellä (https://en.wikipedia.org/wiki/Pivot_point).
			Vector2 offset = new Vector2(_width * _cellSize.X / 2, _height * _cellSize.Y / 2);

			// Lataa Cell-scene. Luomme tästä uuden olion kutakin ruutua kohden.
			PackedScene cellScene = ResourceLoader.Load<PackedScene>(_cellScenePath);
			if (cellScene == null)
			{
				GD.PrintErr("Cell sceneä ei löydy! Gridiä ei voi luoda!");
				return;
			}

			// Alustetaan Grid kahdella sisäkkäisellä for-silmukalla.
			for (int x = 0; x < _width; ++x)
			{
				for (int y = 0; y < _height; ++y)
				{
					// Luo uusi olio Cell-scenestä.
					Cell cell = cellScene.Instantiate<Cell>();
					// Lisää juuri luotu Cell-olio gridin Nodepuuhun.
					AddChild(cell);

					// TODO: Laske ja aseta ruudun sijainti niin maailman koordinaatistossa kuin
					// ruudukonkin koordinaatistossa. Aseta ruudun sijainti käyttäen cell.Position propertyä.
                    cell.Position = new Vector2(x * _cellSize.X, y * _cellSize.Y) - offset;

                    // TODO: Tallenna ruutu tietorakenteeseen oikealle paikalle.
                    _cells[x, y] = cell;
				}
			}
		}
	}
}
