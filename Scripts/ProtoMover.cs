using Godot;
using System;

namespace SnekGame
{
    public partial class ProtoMover : CharacterBody2D
    {
        [Export] private float _speed = 100.0f;

        public void GetInput()
        {
            Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
            Velocity = inputDirection * _speed;
        }

        public override void _PhysicsProcess(double delta)
        {
            GetInput();
            MoveAndCollide(Velocity * (float)delta);
        }
    }
}
