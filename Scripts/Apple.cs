using Godot;
using System;

namespace SnekGame
{
    public partial class Apple : Sprite2D
    {
        [Export] private float _speed = 100.0f;
        // Set the position ot move to
        [Export] public Vector2 destination = new Vector2(110, 110);
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            // Set the position of the apple
            Position = new Vector2(10, 10);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            // move the apple back and forth between the two points
            Position = new Vector2(Position[0] + _speed * (float)delta, Position[1]);
            if (Position[0] > destination[0])
            {
                _speed = -_speed;
            }
            if (Position[0] < 10)
            {
                _speed = -_speed;
            }
            
        }
    }
}
