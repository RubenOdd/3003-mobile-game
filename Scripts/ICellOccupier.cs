namespace SnakeGame
{
	public interface ICellOccupier
	{
		CellOccupierType Type { get; }
	}

	public enum CellOccupierType
	{
		None,
		Snake,
		Collectable,
		Obstacle,
	}
}