using Godot;

namespace SnakeGame
{
	public partial class Apple : Collectable
	{
		[Export] private int _score = 10;

		public override void Collect(Snek snake)
		{
			// TODO: Kasvata matoa

			// Pisteitä ylläpidetään Levelissä. Kasvata Scorea _scoren verran.
			Level.Current.Score += _score;

			GD.Print("Apple collected!");

			// Korvaa omena uudella eri sijainnissa
			Level.Current.ReplaceApple();
		}
	}
}
