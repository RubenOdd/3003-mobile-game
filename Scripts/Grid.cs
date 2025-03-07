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

        public int Width => _width;
        public int Height => _height;

		// Tähän 2-uloitteiseen taulukkoon on tallennettu gridin solut. Alussa taulukkoa ei ole, vaan
		// muuttujassa on tyhjä viittaus (null). Taulukko pitää luoda pelin alussa (esim. _Ready-metodissa).
		private Cell[,] _cells = null;

        // Ne solut, joissa ei ole mitään olioita.
		private List<Cell> _freeCells = new List<Cell>();

		public override void _Ready()
		{
            _cells = new Cell[Width, Height];

			Vector2 offset = new Vector2(_width * _cellSize.X / 2, _height * _cellSize.Y / 2);
            Vector2 halfNode = new Vector2(_cellSize.X / 2f, _cellSize.Y / 2f);

			offset.X -= halfNode.X;
			offset.Y -= halfNode.Y;

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
                    cell.GridPosition = new Vector2I(x, y);

                    // TODO: Tallenna ruutu tietorakenteeseen oikealle paikalle.
                    _cells[x, y] = cell;
                    _freeCells.Add(cell);
				}
			}
		}

        /// <summary>
		/// Palauttaa gridin sijaintia vastaavan maailman koordinaatin
		/// </summary>
		/// <param name="gridPosition">Sijainti gridillä</param>
		/// <param name="worldPosition">Out-parametri, johon asetetaan maailman koordinaatti</param>
		/// <returns>True, jos gridPositon on laillinen koordinaatti, false muuten.</returns>
        public bool GetWorldPosition(Vector2I gridPosition, out Vector2 worldPosition)
		{
			if (IsInvalidCoordinate(gridPosition))
			{
				worldPosition = Vector2.Zero;
				// Koordinaatti ei ole gridillä
				return false;
			}

			// Olettaa Gridin olevan 0,0 koordinaatissa. Voi käyttää myös GlobalPositionia.
			worldPosition = _cells[gridPosition.X, gridPosition.Y].Position;
			return true;
		}

		private bool IsInvalidCoordinate(Vector2I gridPosition)
		{
			return gridPosition.X < 0 || gridPosition.Y < 0
				|| gridPosition.X >= Width || gridPosition.Y >= Height;
		}


        /// <summary>
		/// Varaa solun occupier-oliolle.
		/// </summary>
		/// <param name="occupier">Solun varaava olio</param>
		/// <param name="gridPosition">Solun sijainti koordinaatistossa.</param>
		/// <returns>True, jos varaaminen onnistuu. False muuten.</returns>
        public bool OccupyCell(ICellOccupier occupier, Vector2I gridPosition)
		{
			if (IsInvalidCoordinate(gridPosition))
			{
				return false;
			}

			Cell cell = _cells[gridPosition.X, gridPosition.Y];
			bool canOccupy = cell.Occupy(occupier);
			if (canOccupy)
			{
				// Solu on varattu, poista se _freeCells-listalta.
				_freeCells.Remove(cell);
			}

            GD.Print("Occupied cell at " + gridPosition);

			return canOccupy;
		}

        /// <summary>
		/// Vapauttaa solun.
		/// </summary>
		/// <param name="gridPosition">Solun sijainti gridin koordinaatistossa.</param>
		/// <returns>True, jos vapautus onnistuu. False muuten.</returns>
        public bool ReleaseCell(Vector2I gridPosition)
		{
			if (IsInvalidCoordinate(gridPosition))
			{
				return false;
			}

			Cell cell = _cells[gridPosition.X, gridPosition.Y];
			cell.Release();
			_freeCells.Add(cell);

			return true;
		}

		/// <summary>
		/// Palauttaa satunnaisen vapaan solun gridiltä.
		/// </summary>
		/// <returns>Satunnainen vapaa solu</returns>
		public Cell GetRandomFreeCell()
		{
			// Koska lista indeksoidaan C#-kielessä nollasta alkaen, pitää listan
			// pituudesta vähentää yksi, jotta saadaan oikeat luvut mukaan randomiin.
			int randomIndex = GD.RandRange(0, _freeCells.Count - 1);
			return _freeCells[randomIndex];
		}

        /// <summary>
		/// Returns true, if there is a collectable in the coordinate "gridPosition".
		/// </summary>
		/// <param name="gridPosition">A coordinate on a grid.</param>
		/// <returns>True, if there is a collectable in the coordinate "gridPosition".
		/// False otherwise.</returns>
		public bool HasCollectable(Vector2I gridPosition)
		{
			if (IsInvalidCoordinate(gridPosition))
			{
				// Koordinaatti ei ole laillinen
				return false;
			}

			Cell cell = _cells[gridPosition.X, gridPosition.Y];
			return cell.Occupier.Type == CellOccupierType.Collectable;
		}

		/// <summary>
		/// Returns a Collectable object if cell has one.
		/// </summary>
		/// <param name="gridPosition">Coordinate of the cell</param>
		/// <returns>Collectable, if cell has one. Null otherwise.</returns>
		public Collectable GetCollectable(Vector2I gridPosition)
		{
			if (IsInvalidCoordinate(gridPosition))
			{
				// Koordinaatti ei ole laillinen
				return null;
			}

			Cell cell = _cells[gridPosition.X, gridPosition.Y];
			// Tarkista, onko Occupier Collectable
			if (cell.Occupier is Collectable)
			{
				// Jos on, palauta se Collectable-tyyppisenä.
				return cell.Occupier as Collectable;
			}

			// Muussa tapauksessa palauta tyhjä viittaus.
			return null;
		}

        /// <summary>
        /// Checks if the gridPosition cellis free.
        /// </summary>
        /// <param name="gridPosition">Grid coordinate</param>
        /// <returns>True if cell is free, otherwise False</returns>
        public bool IsFree(Vector2I gridPosition)
        {
            if (IsInvalidCoordinate(gridPosition))
            {
                return false;
            }

            return _cells[gridPosition.X, gridPosition.Y].IsFree;
        }
	}
}
