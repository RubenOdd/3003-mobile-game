using Godot;
using System;

namespace SnekGame
{
    public partial class Apple : Sprite2D
    {
        [Export] private float _speed = 100.0f;
        // Set the position ot move to
        [Export] public Vector2 _destination = Vector2.Zero;

        public override void _Ready()
        {
            // Set the position of the apple
            // Position = new Vector2(10, 10);
            _destination = _destination.Normalized();
        }

        public override void _Process(double delta)
        {
            // move the apple back and forth between the two points
            // Position = new Vector2(Position[0] + _speed * (float)delta, Position[1]);
            // if (Position[0] > _destination[0])
            // {
            //     _speed = -_speed;
            // }
            // if (Position[0] < 10)
            // {
            //     _speed = -_speed;
            // }

            // move the apple back and forth between the start and end points
            Vector2 movement = _destination * _speed * (float)delta;
            GlobalPosition += movement;
        }
    }
}
