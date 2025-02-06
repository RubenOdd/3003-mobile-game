using Godot;
using System;

namespace SnekGame
{
    public partial class Snek : CharacterBody2D
    {
        [Export] private float _speed = 100.0f;
        [Export] private int _tileSize = 16;
        Vector2 _direction = Vector2.Zero;
        bool moving = false;

        public void GetInput()
        {
            if (Input.IsActionJustPressed("left"))
            {
                _direction = Vector2.Left;
                Move();
            }
            else if (Input.IsActionJustPressed("right"))
            {
                _direction = Vector2.Right;
                Move();
            }
            else if (Input.IsActionJustPressed("up"))
            {
                _direction = Vector2.Up;
                Move();
            }
            else if (Input.IsActionJustPressed("down"))
            {
                _direction = Vector2.Down;
                Move();
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

        private void Move()
        {
            if (_direction != Vector2.Zero)
            {
                if (moving == false)
                {
                    moving = true;
                    Tween tween = CreateTween();
                    tween.TweenProperty(this, "position", Position + _direction * _tileSize, 1 / _speed);
                    MoveFalse();
                }
            }
        }

        private void MoveFalse()
        {
            moving = false;
        }
    }
}
