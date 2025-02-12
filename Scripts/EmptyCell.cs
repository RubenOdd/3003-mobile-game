namespace SnakeGame
{
	// Kuvaa tyhjää solua Gridillä.
	public class EmptyCell : ICellOccupier
	{
		public CellOccupierType Type => CellOccupierType.None;
	}
}