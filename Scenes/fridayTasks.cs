using Godot;
using System;

public partial class fridayTasks : Node2D
{
    int n1 = 0;
    int n2 = 1;
    int result = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GD.Print("Hello World!");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        if(n1 == 0 && n2 == 1) {
            GD.Print(n1);
            GD.Print(n2);
        }
        if(result <= 1000) {
            result = n1 + n2;
            GD.Print(result);
            n1 = n2;
            n2 = result;
        }
	}
}
