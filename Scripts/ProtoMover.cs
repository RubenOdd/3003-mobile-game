using Godot;
using System;

namespace SnekGame
{
    public partial class ProtoMover : CharacterBody2D
    {
        [Export] private float _speed = 100.0f;
        [Export(PropertyHint.Enum, "0,1")] private int _direction;

        public override void _Ready()
        {
            GlobalPosition = new Vector2(0,0);
        }

        public override void _Process(double delta)
        {
            if (_direction == 0) 
            {
                GlobalPosition += Vector2.Right * (float)delta * _speed;
            }
            else if (_direction == 1)
            {
                GlobalPosition += Vector2.Down * (float)delta * _speed;
            }
        }

    }
}
