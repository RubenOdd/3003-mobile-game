using Godot;
using System;

namespace SnakeGame
{
    public partial class SaveData : Node
    {
        public override void _Ready()
        {
            Godot.Collections.Dictionary data = new Godot.Collections.Dictionary();

            data.Add("name", "Snake");

            string json = Json.Stringify(data);

            GD.Print(json);
        }

        public void SaveTextToFile()
        {
            
        }
    }
}
