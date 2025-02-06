using Godot;
using System;

namespace SnekGame
{
    public partial class Snek : CharacterBody2D
    {
        [Export] private float _speed = 100.0f;
        Vector2 _direction = Vector2.Zero;

        public void GetInput()
        {
            Vector2 inputDirection = Input.GetVector("left", "right", "up", "down");
            if (Input.IsActionPressed("left") || Input.IsActionPressed("right"))
            {
                inputDirection.Y = 0;
            }
            else if (Input.IsActionPressed("up") || Input.IsActionPressed("down"))
            {
                inputDirection.X = 0;
            }
            
            Velocity = inputDirection.Normalized() * _speed;

            // rotate the sprite to face the direction of movement
            if (inputDirection != Vector2.Zero)
            {
                _direction = inputDirection;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            GetInput();
            MoveAndCollide(Velocity * (float)delta);

            if (_direction != Vector2.Zero)
            {
                // rotates the sprite to face wrong direction for some reason
                // f it, gonna leave it to friday lesson
                Rotation = _direction.Angle();
            }
        }
    }
}
