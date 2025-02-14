using Godot;

namespace SnakeGame
{
	public partial class Nuclear : Collectable
	{
        // Removes the snake from the scene when collected
        public override void Collect(Snek snake)
        {
            Level.Current.DestroySnake();
            GD.Print("SNAKE IS DEAD!");
        }
	}
}