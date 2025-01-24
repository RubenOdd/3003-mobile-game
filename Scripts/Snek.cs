using Godot;
using System;
using System.Numerics;

namespace SnekGame
{
    public partial class Snek : CharacterBody2D
    {
        [Export] private float _speed = 100.0f;

        public override void _Ready()
        {
            GlobalPosition = new Godot.Vector2(0, 0);
        }

        public override void _Process(double delta)
        {
            GlobalPosition += Godot.Vector2.Down * (float)delta * _speed;
        }

        public void Move(Godot.Vector2 direction, float delta)
        {

        }
    }
}
