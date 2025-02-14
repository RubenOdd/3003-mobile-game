using Godot;
using System;

namespace SnekGame
{
    public partial class FridayTasks : Node2D
    {
        private int _n1 = 0;
        private int _n2 = 1;
        private int _result = 0;
        private int _frameSeq = 1;
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GD.Print("Hello World!");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            if(_n1 == 0 && _n2 == 1) 
            {
                GD.Print($"Frame {_frameSeq}: {_n1}");
                _frameSeq++;
                GD.Print($"Frame {_frameSeq}: {_n2}");
                _frameSeq++;
            }
            
            if(_result <= 1000) 
            {
                _result = _n1 + _n2;
                GD.Print($"Frame {_frameSeq}: {_result}");
                _n1 = _n2;
                _n2 = _result;

                _frameSeq++;
            }
        }
    }
}
