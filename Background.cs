using Godot;
using System;

public partial class Background : Node
{
	// Called when the node enters the scene tree for the first time.
	PlayerChase player;

	public override void _Ready()
	{
		player = GetNode<PlayerChase>("PlayerChase");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionPressed("debug_close")) GetTree().Quit();
	}
}
