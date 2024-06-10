using General;
using Godot;
using System;
using System.ComponentModel.DataAnnotations;
using static General.CharacterMovement;


public partial class PlayerChase : Area2D
{
	// we'll create a parallax animation that will be constantly running in the background
	// the player can boost forward but then it pushes them back (similar to jumping)
	// otherwise changes in the x direction are locked
	// we should have an idle bobbing of the character while moving in a straight line
	
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		Vector2 startPos = new Vector2(Position.X - 50, 100);
		Position = startPos;
		movement = new CharacterMovement(startPos, 400, 400);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(delta > 0.0065) {
			if(delta > 0.007) delta = 0.007;
			InputHandling();
			movement.Update((float)delta);
			Position = movement.GetPos();
		}
	}

	private void InputHandling() {
		if(Input.IsActionJustReleased("move_up")) {
			movement.ReleaseY();
		}
		if(Input.IsActionJustReleased("move_down")) {
			movement.ReleaseY();
		}
		if(Input.IsActionPressed("move_up")) {
			movement.MoveUp();
		}
		if(Input.IsActionPressed("move_down")) {
			movement.MoveDown();
		}
		if(Input.IsActionJustPressed("boost")) {
			movement.Boost();
		}
	}

	private CharacterMovement movement;
}
